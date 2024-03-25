using System.Text;

namespace MoreMountains.Feedbacks
{
	public class MMFloatingTextImages : MMFloatingTextMeshPro
	{
		readonly StringBuilder _stringBuilder = new();
	    
	    // GENERALIZE: hardcode to use other TMP Sprite Assets if needed
		public override void SetText(string newValue)
		{
			_stringBuilder.Clear();

			foreach (char digitChar in newValue)
			{
				if (int.TryParse(digitChar.ToString(), out int digitInt))
				{
					_stringBuilder.Append($"<sprite=\"TwemojiNumbers\" index={digitInt}>");
				}
				else
				{
					_stringBuilder.Append(digitChar);
				}
			}
			
			base.SetText(_stringBuilder.ToString());
		}
    }
}