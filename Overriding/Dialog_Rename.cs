using AlcoholV.Extension;
using UnityEngine;
using Verse;

namespace AlcoholV.Overriding
{
    public class Dialog_Rename : Window
    {
        private readonly IExtendable _iExtendable;
        private string _curName;

        public Dialog_Rename(IExtendable iExtendable)
        {
            _iExtendable = iExtendable;
            _curName = iExtendable.Name;
            closeOnEscapeKey = true;
            absorbInputAroundWindow = true;
        }

        protected virtual int MaxNameLength => 32;

        public override Vector2 InitialSize => new Vector2(400f, 175f);

        protected void SetName(string name)
        {
            _iExtendable.Name = name;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            var flag = false;
            if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Return))
            {
                flag = true;
                Event.current.Use();
            }
            Widgets.Label(new Rect(0f, 5f, inRect.width, 35f), "Rename".Translate());

            var text = Widgets.TextField(new Rect(0f, 40f, inRect.width, 35f), _curName);
            if (text.Length < MaxNameLength)
                _curName = text;
            if (Widgets.ButtonText(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK", true, false, true) || flag)
            {
                var acceptanceReport = NameIsValid(_curName);
                if (!acceptanceReport.Accepted)
                {
                    if (acceptanceReport.Reason == null)
                        Messages.Message("NameIsInvalid".Translate(), MessageSound.RejectInput);
                    else
                        Messages.Message(acceptanceReport.Reason, MessageSound.RejectInput);
                }
                else
                {
                    SetName(_curName);
                    Find.WindowStack.TryRemove(this, true);
                }
            }
        }

        protected AcceptanceReport NameIsValid(string name)
        {
            if (name.Length == 0)
            {
                Messages.Message("NameIsInvalid".Translate(), MessageSound.RejectInput);
                return false;
            }
            Messages.Message("BillGainsName".Translate(new object[]
                {
                    _curName
                }), MessageSound.Benefit);
            return true;
        }
    }
}