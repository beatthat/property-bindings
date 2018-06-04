
namespace BeatThat
{
	public abstract class HasBool : HasValue, IHasBool
	{
		public abstract bool value { get; set; }

		public override object valueObj { get { return this.value; } }
	}
}
