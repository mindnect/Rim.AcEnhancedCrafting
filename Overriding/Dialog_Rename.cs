using UnityEngine;
using Verse;

namespace AlcoholV.Overriding
{
    public class Dialog_Rename : Window
    {
        private readonly IExtendable _iExtendable;
        private string _curName;

        protected virtual int MaxNameLength => 32;

        public Dialog_Rename(IExtendable iExtendable)
        {
            _iExtendable = iExtendable;
            _curName = iExtendable.Name;
            closeOnEscapeKey = true;
            absorbInputAroundWindow = true;
        }

        public override Vector2 InitialSize => new Vector2(400f, 175f);
        protected void SetName(string name)
        {
            _iExtendable.Name = name;
        }

        public override void DoWindowContents(Rect inRect)
        {
          	Text.Font = GameFont.Small;
			bool flag = false;
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
			{
				flag = true;
				Event.current.Use();
			}
            Widgets.Label(new Rect(0f, 5f, inRect.width, 35f), "AlcoholV.Rename".Translate());

            string text = Widgets.TextField(new Rect(0f, 40f, inRect.width, 35f), this._curName);
			if (text.Length < this.MaxNameLength)
			{
				this._curName = text;
			}
			if (Widgets.ButtonText(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK", true, false, true) || flag)
			{
				AcceptanceReport acceptanceReport = this.NameIsValid(this._curName);
				if (!acceptanceReport.Accepted)
				{
					if (acceptanceReport.Reason == null)
					{
						Messages.Message("NameIsInvalid".Translate(), MessageSound.RejectInput);
					}
					else
					{
						Messages.Message(acceptanceReport.Reason, MessageSound.RejectInput);
					}
				}
				else
				{
					this.SetName(this._curName);
					Find.WindowStack.TryRemove(this, true);
				}
			}
         
        }

        protected AcceptanceReport NameIsValid(string name)
        {
            if (name.Length == 0)
            {
                Messages.Message("AlcoholV.InvalidName".Translate(), MessageSound.RejectInput);
                return false;
            }
            Messages.Message("AlcoholV.Renamed".Translate(), MessageSound.Benefit);
            return true;
        }
    }
}