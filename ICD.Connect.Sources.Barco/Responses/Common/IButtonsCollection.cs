using System.Collections.Generic;

namespace ICD.Connect.Sources.Barco.Responses.Common
{
	public interface IButtonsCollection
	{
		IEnumerable<Button> GetButtons();

		void AddButton(Button value);
	}
}
