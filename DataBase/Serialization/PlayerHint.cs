using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using LabApi.Features.Wrappers;
using System.Collections.Generic;
using System;

namespace EasyTools.DataBase.Serialization
{
    public struct PlayerHint : IDisposable
    {
        private readonly List<Hint> _hints = new();

        private Hint hint_914;
        private Hint hint_elevator;

        public PlayerHint(Player player, HintData data_914, HintData data_elevator)
        {
            hint_914 = new Hint
            {
                Text = "",
                XCoordinate = data_914.x,
                YCoordinate = data_914.y,
                FontSize = data_914.font,
                YCoordinateAlign = HintVerticalAlign.Bottom
            };

            hint_elevator = new Hint
            {
                Text = "",
                XCoordinate = data_elevator.x,
                YCoordinate = data_elevator.y,
                FontSize = data_elevator.font,
                YCoordinateAlign = HintVerticalAlign.Bottom
            };

            var display = PlayerDisplay.Get(player);
            _hints.AddRange(new[] { hint_914, hint_elevator });
            _hints.ForEach(display.AddHint);
        }

        public void Show914(string text)
        {
            hint_914.Text = text;
            hint_914.Hide = false;
            hint_914.HideAfter(15f);
        }

        public void ShowElevator(string text)
        {
            hint_elevator.Text = text;
            hint_elevator.Hide = false;
            hint_elevator.HideAfter(7f);
        }

        public void Start() { }

        public void Dispose()
        {
            foreach (var h in _hints) h.Hide = true;
        }
    }
}
