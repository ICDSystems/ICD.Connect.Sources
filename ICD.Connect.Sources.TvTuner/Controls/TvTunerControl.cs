using ICD.Connect.Sources.TvTuner.Devices;

namespace ICD.Connect.Sources.TvTuner.Controls
{
    public sealed class TvTunerControl : AbstractTvTunerControl<ITvTuner>
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
	    public TvTunerControl(ITvTuner parent, int id)
			: base(parent, id)
	    {
	    }

	    #region Methods

	    /// <summary>
	    /// Goes to the given channel number.
	    /// </summary>
	    /// <param name="number"></param>
		public override void SetChannel(string number)
	    {
		    Parent.SetChannel(number);
	    }

	    /// <summary>
	    /// Goes to the next channel.
	    /// </summary>
		public override void ChannelUp()
	    {
		    Parent.ChannelUp();
	    }

	    /// <summary>
	    /// Goes to the previous channel.
	    /// </summary>
		public override void ChannelDown()
	    {
		    Parent.ChannelDown();
	    }

	    /// <summary>
	    /// Sends a single number to the tuner for manual channel selection.
	    /// </summary>
	    /// <param name="number"></param>
		public override void SendNumber(char number)
	    {
		    Parent.SendNumber(number);
	    }

	    #endregion
    }
}
