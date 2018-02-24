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

		#region Channels

		/// <summary>
		/// Goes to the given channel number.
		/// </summary>
		/// <param name="number"></param>
		public override void SetChannel(string number)
		{
			Parent.SetChannel(number);
		}

		/// <summary>
		/// Sends a single number to the tuner for manual channel selection.
		/// </summary>
		/// <param name="number"></param>
		public override void SendNumber(char number)
		{
			Parent.SendNumber(number);
		}

		/// <summary>
		/// Sends the clear command.
		/// </summary>
		public override void Clear()
		{
			Parent.Clear();
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

		#endregion

		#region Playback

		/// <summary>
		/// Sends the repeat command.
		/// </summary>
		public override void Repeat()
		{
			Parent.Repeat();
		}

		/// <summary>
		/// Sends the rewind command.
		/// </summary>
		public override void Rewind()
		{
			Parent.Rewind();
		}

		/// <summary>
		/// Sends the fast-forward command.
		/// </summary>
		public override void FastForward()
		{
			Parent.FastForward();
		}

		/// <summary>
		/// Sends the stop command.
		/// </summary>
		public override void Stop()
		{
			Parent.Stop();
		}

		/// <summary>
		/// Sends the play command.
		/// </summary>
		public override void Play()
		{
			Parent.Play();
		}

		/// <summary>
		/// Sends the pause command.
		/// </summary>
		public override void Pause()
		{
			Parent.Pause();
		}

		/// <summary>
		/// Sends the record command.
		/// </summary>
		public override void Record()
		{
			Parent.Record();
		}

		#endregion

		#region Menus

		/// <summary>
		/// Goes to the above page.
		/// </summary>
		public override void PageUp()
		{
			Parent.PageUp();
		}

		/// <summary>
		/// Goes to the below page.
		/// </summary>
		public override void PageDown()
		{
			Parent.PageDown();
		}

		/// <summary>
		/// Sends the top-menu command.
		/// </summary>
		public override void TopMenu()
		{
			Parent.TopMenu();
		}

		/// <summary>
		/// Sends the popup-menu command.
		/// </summary>
		public override void PopupMenu()
		{
			Parent.PopupMenu();
		}

		/// <summary>
		/// Sends the return command.
		/// </summary>
		public override void Return()
		{
			Parent.Return();
		}

		/// <summary>
		/// Sends the info command.
		/// </summary>
		public override void Info()
		{
			Parent.Info();
		}

		/// <summary>
		/// Sends the eject command.
		/// </summary>
		public override void Eject()
		{
			Parent.Eject();
		}

		/// <summary>
		/// Sends the power command.
		/// </summary>
		public override void Power()
		{
			Parent.Power();
		}

		/// <summary>
		/// Sends the red command.
		/// </summary>
		public override void Red()
		{
			Parent.Red();
		}

		/// <summary>
		/// Sends the green command.
		/// </summary>
		public override void Green()
		{
			Parent.Green();
		}

		/// <summary>
		/// Sends the yellow command.
		/// </summary>
		public override void Yellow()
		{
			Parent.Yellow();
		}

		/// <summary>
		/// Sends the blue command.
		/// </summary>
		public override void Blue()
		{
			Parent.Blue();
		}

		/// <summary>
		/// Sends the up command.
		/// </summary>
		public override void Up()
		{
			Parent.Up();
		}

		/// <summary>
		/// Sends the down command.
		/// </summary>
		public override void Down()
		{
			Parent.Down();
		}

		/// <summary>
		/// Sends the left command.
		/// </summary>
		public override void Left()
		{
			Parent.Left();
		}

		/// <summary>
		/// Sends the right command.
		/// </summary>
		public override void Right()
		{
			Parent.Right();
		}

		/// <summary>
		/// Sends the select command.
		/// </summary>
		public override void Select()
		{
			Parent.Select();
		}

		/// <summary>
		/// Sends the enter command.
		/// </summary>
		public override void Enter()
		{
			Parent.Enter();
		}

		#endregion
	}
}
