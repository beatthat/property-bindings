using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace BeatThat
{
	public struct TypeAndDrivenType
	{
		public Type type;
		public Type drivenType;
	}

	public static class FindTargetPropertyWrapperTypes 
	{
		#if Debug_FindTargetPropertyWrapperTypes
		public static bool m_debug = true;
		#else 
		public static bool m_debug = false;
		#endif

		public static void FindWrapperTypes<ValType>(Type drivenType, IList<Type> results)
		{
			FindWrapperTypes (drivenType, typeof(ValType), results);
		}

		/// <summary>
		/// Searches assemblies for all components that can provide IHasValue for a specificed value type
		/// while also driving a specificed driven type.
		/// 
		/// As an illustrating example for where this is useful, 
		/// say you have a component SetColor that needs an IHasValue<Color> to target.
		/// You add SetColor to a GameObject that has an Image component.
		/// The SetColor can't target Image directly, but maybe we have in a class GraphicColor which implements IHasValue<Color> and IDrive<Graphic>.
		/// 
		/// If you call <code></code>FindWrapperTypes(drivenType: typeof(Image), valueType: typeof(Color), results)
		/// that should find and return GraphicColor.
		/// 
		/// </summary>
		/// <param name="drivenType">Driven type.</param>
		/// <param name="valueType">Value type.</param>
		/// <param name="results">Results.</param>
		public static void FindWrapperTypes(Type drivenType, Type valueType, IList<Type> results)
		{
			if (!m_hasLoadedWrapperTypes) {
				LoadWrapperTypes ();
			}

			foreach (var kv in m_wrapperTypesByDrivenType) {

				if (!kv.Key.IsAssignableFrom(drivenType)) {
					continue;
				}
				foreach (var t in kv.Value) {

					if (!typeof(IHasValue).IsAssignableFrom (t)) {
						#if UNITY_EDITOR || DEBUG_UNSTRIP
						if (m_debug) {
							Debug.Log ("FindTargetPropertyWrapperTypes - type " + t.Name + " is not assignable from IHasValue");
						}
						#endif
						continue;
					}


					var iHasValueInterface = Array.Find(t.GetInterfaces(), intf => intf.IsGenericType && typeof(IHasValue<>).IsAssignableFrom(intf.GetGenericTypeDefinition()));

					if (iHasValueInterface == null) {
						continue;
					}

					if (iHasValueInterface.ContainsGenericParameters) {
						continue;
					}


					var genArgs = iHasValueInterface.GetGenericArguments ();
					if (genArgs == null || genArgs.Length < 1) {
						continue;
					}
						
					if (genArgs.Length != 1 || !genArgs [0].IsAssignableFrom (valueType)) {
						#if UNITY_EDITOR || DEBUG_UNSTRIP
						if (m_debug) {
							Debug.Log ("FindTargetPropertyWrapperTypes - type " + t.Name 
								+ " is not assignable from IHasValue<" + valueType.Name 
								+ "> closest matching interface is IHasValue<" + genArgs[0].Name + ">");
						}
						#endif
						continue;
					}

					results.Add (t);
				}

				#if UNITY_EDITOR || DEBUG_UNSTRIP
				if (m_debug) {
					Debug.Log ("FindTargetPropertyWrapperTypes - driven type=" + drivenType.Name
					+ " searching wrappers that implement IHasValue<" + typeof(ValueType).Name + "> and..."
					+ typeof(ValueType).Name
					+ (results.Count > 0 ? 
							(" found wrapper types: " + string.Join (",", results.Select (wt => wt.Name).ToArray ())) :
							(" did not find any wrapper types")
					)
					);
				}
				#endif
			}
		}
			

		private static void LoadWrapperTypes()
		{
			m_hasLoadedWrapperTypes = true;

			m_wrapperTypesByDrivenType = new Dictionary<Type, IList<Type>>();

			var drivers = new List<TypeAndDrivenType> (); 

			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
				foreach (Type t in a.GetTypes()) {
					if (!typeof(IDrive).IsAssignableFrom (t)) {
						continue;
					}

					var iDriveInterface = Array.Find(t.GetInterfaces(), intf => intf.IsGenericType && typeof(IDrive<>).IsAssignableFrom(intf.GetGenericTypeDefinition()));

					if (iDriveInterface == null) {
						continue;
					}

					if (iDriveInterface.ContainsGenericParameters) {
						continue;
					}


					var genArgs = iDriveInterface.GetGenericArguments ();
					if (genArgs == null || genArgs.Length < 1) {
						continue;
					}

					drivers.Add (new TypeAndDrivenType {
						type = t,
						drivenType = genArgs[0]
					});
						
					#if UNITY_EDITOR || DEBUG_UNSTRIP
					if(m_debug) {
						Debug.Log("FindTargetPropertyWrapperTypes - found driver type " + t.Name + " that drives type " + genArgs[0].Name);
					}
					#endif
				}
			}

			foreach (var d in drivers) {
				IList<Type> wrapperTypes;
				if (!m_wrapperTypesByDrivenType.TryGetValue (d.drivenType, out wrapperTypes)) {
					wrapperTypes = new List<Type> ();
					m_wrapperTypesByDrivenType [d.drivenType] = wrapperTypes;
				}
				wrapperTypes.Add (d.type);
			}
		}

		private static bool m_hasLoadedWrapperTypes;
		private static Dictionary<Type, IList<Type>> m_wrapperTypesByDrivenType;
	}
}