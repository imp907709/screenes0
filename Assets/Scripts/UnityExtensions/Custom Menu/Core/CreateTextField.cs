using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityExtensions.Custom_Menu.Core
{
    public class CreateTextField
    {
        public static TextField CreateFloat(float _scale, string _text)
        {
            var scaleField = new TextField(_text);
            scaleField.value = _scale.ToString(CultureInfo.InvariantCulture);
            scaleField.RegisterValueChangedCallback(evt =>
            {
                float.TryParse(evt.newValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _scale);
            });

            return scaleField;
        }
        
        public static TextField CreateInt(int _scale, string _text)
        {
            var scaleField = new TextField(_text);
            scaleField.value = _scale.ToString(CultureInfo.InvariantCulture);
            scaleField.RegisterValueChangedCallback(evt =>
            {
                int.TryParse(evt.newValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out _scale);
            });

            return scaleField;
        }

        public static int ParseIntField(TextField field, int fallback, int min, int max)
        {
            if (field == null ||
                !int.TryParse(field.value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v))
                v = fallback;
            return Mathf.Clamp(v, min, max);
        }

        public static float ParseFloatField(TextField field, float fallback, float min, float max)
        {
            if (field == null ||
                !float.TryParse(field.value, NumberStyles.Float, CultureInfo.InvariantCulture, out float v))
                v = fallback;
            return Mathf.Clamp(v, min, max);
        }
    }
}