using ICD.Connect.Devices;

namespace ICD.Connect.Sources.TvTuner.AppleTv
{
	public interface IAppleTvDevice : IDevice
	{
		/// <summary>
		/// Sends the UP command.
		/// </summary>
		void Up();

		/// <summary>
		/// Sends the DOWN command.
		/// </summary>
		void Down();

		/// <summary>
		/// Sends the LEFT command.
		/// </summary>
		void Left();

		/// <summary>
		/// Sends the RIGHT command.
		/// </summary>
		void Right();

		/// <summary>
		/// Sends the SELECT command.
		/// </summary>
		void Select();

		/// <summary>
		/// Sends the MENU command.
		/// </summary>
		void Menu();

		/// <summary>
		/// Sends the PLAY/PAUSE toggle command.
		/// </summary>
		void PlayPauseToggle();
	}
}