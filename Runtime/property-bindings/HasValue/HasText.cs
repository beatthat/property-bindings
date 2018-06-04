
namespace BeatThat
{
	public abstract class HasText : HasValue, IHasText
	{
		abstract public string value { get; set; }

		override public object valueObj { get { return this.value; } }
	}
}
