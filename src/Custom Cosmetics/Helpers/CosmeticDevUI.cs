using UnityEngine;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace Custom_Cosmetics.Helpers
{
    public class CosmeticDevUI : MonoBehaviour
    {
        public bool _isVisible = false;
        private Rect _windowRect = new Rect(20, 20, 360, 500);
        private Vector2 _scrollPos;
        private string _selectedTab = "Head";

        private class FloatInputState
        {
            public string text;
            public float value;
            public bool isFocused;
            public int controlId;
        }

        private Dictionary<string, FloatInputState> _inputStates;
        private int _currentTabIndex = 0;
        private int _nextControlId = 1000;

        private GameObject _headObj, _bodyObj, _faceObj;

        private void Awake()
        {
            _inputStates = new Dictionary<string, FloatInputState>();
        }

        private void Update()
        {
            if (!Plugin.CosmeticDevTools.Value)
            {
                _isVisible = false;
                return;
            }

            FetchCurrentCostumeObjects();

            UpdateInputStates();
        }

        private void UpdateInputStates()
        {
            GameObject[] targets = { _headObj, _bodyObj, _faceObj };
            string[] tabNames = { "Head", "Body", "Face" };
            string[] transformTypes = { "Pos", "Rot", "Scale" };
            string[] axes = { "X", "Y", "Z" };

            for (int tabIdx = 0; tabIdx < 3; tabIdx++)
            {
                var target = targets[tabIdx];
                if (target == null) continue;

                Vector3 pos = target.transform.localPosition;
                Vector3 rot = target.transform.localEulerAngles;
                Vector3 scale = target.transform.localScale;

                Vector3[] values = { pos, rot, scale };

                for (int tIdx = 0; tIdx < 3; tIdx++)
                {
                    for (int aIdx = 0; aIdx < 3; aIdx++)
                    {
                        string key = $"{tabNames[tabIdx]}_{transformTypes[tIdx]}_{axes[aIdx]}";
                        if (!_inputStates.ContainsKey(key))
                        {
                            _inputStates[key] = new FloatInputState { controlId = _nextControlId++ };
                        }

                        if (!_inputStates[key].isFocused)
                        {
                            _inputStates[key].value = values[tIdx][aIdx];
                        }
                    }
                }
            }
        }

        private void FetchCurrentCostumeObjects()
        {
            if (GameUtil.TryGetLocalPlayer(out var player))
            {
                var manager = player.GetObject<PlayerCostumeManager>();
                if (manager != null && manager.costumes != null && manager.currentCostumeID < manager.costumes.Length)
                {
                    var costume = manager.costumes[manager.currentCostumeID];
                    if (costume != null && costume.name.StartsWith("custom-costume-"))
                    {
                        string id = costume.name.Replace("custom-costume-", "");
                        _headObj = GameObject.Find($"custom-costume-{id}-head");
                        _bodyObj = GameObject.Find($"custom-costume-{id}-body");
                        _faceObj = GameObject.Find($"custom-costume-{id}-face");
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (!_isVisible) return;

            _windowRect = GUI.Window(999, _windowRect, DrawWindow, "Cosmetic DevTools");
        }

        private void DrawWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 25));

            GUILayout.BeginHorizontal();

            GUI.backgroundColor = _selectedTab == "Head" ? Color.cyan : Color.gray;
            if (GUILayout.Button("HEAD", GUILayout.Height(30)))
            {
                _selectedTab = "Head";
                _currentTabIndex = 0;
            }

            GUI.backgroundColor = _selectedTab == "Body" ? Color.cyan : Color.gray;
            if (GUILayout.Button("BODY", GUILayout.Height(30)))
            {
                _selectedTab = "Body";
                _currentTabIndex = 1;
            }

            GUI.backgroundColor = _selectedTab == "Face" ? Color.cyan : Color.gray;
            if (GUILayout.Button("FACE", GUILayout.Height(30)))
            {
                _selectedTab = "Face";
                _currentTabIndex = 2;
            }

            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true));

            GameObject target = _selectedTab switch
            {
                "Head" => _headObj,
                "Body" => _bodyObj,
                "Face" => _faceObj,
                _ => null
            };

            if (target != null)
            {
                DrawTransformControls(target);
            }
            else
            {
                GUILayout.Box($"No {_selectedTab} object found for this costume.", GUILayout.Height(50));
                GUILayout.Label("Make sure you're wearing a custom costume that has this part.");
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("LOG TO CONSOLE", GUILayout.Height(30)))
            {
                LogCurrentSettings();
            }

            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }

        private void DrawTransformControls(GameObject obj)
        {
            float posStep = Plugin.SnapStepPos.Value;
            float rotStep = Plugin.SnapStepRot.Value;

            DrawSectionHeader("POSITION", $"Step: {posStep:F3}");
            DrawFloat3Row("Pos", obj.transform, (t, v) => t.localPosition = v, posStep);

            GUILayout.Space(8);

            DrawSectionHeader("ROTATION", $"Step: {rotStep:F1}°");
            DrawFloat3Row("Rot", obj.transform, (t, v) => t.localRotation = Quaternion.Euler(v), rotStep);

            GUILayout.Space(8);

            DrawSectionHeader("SCALE", $"Step: {posStep:F3}°");
            DrawFloat3Row("Scale", obj.transform, (t, v) => t.localScale = v, posStep);
        }

        private void DrawSectionHeader(string title, string info)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(title, GUILayout.ExpandWidth(true));
            GUILayout.Label(info, GUILayout.Width(80));
            GUILayout.EndHorizontal();
        }

        private void DrawFloat3Row(string transformType, Transform target, System.Action<Transform, Vector3> applyChange, float step)
        {
            GUILayout.BeginHorizontal();

            Vector3 currentValue = transformType switch
            {
                "Pos" => target.localPosition,
                "Rot" => target.localEulerAngles,
                "Scale" => target.localScale,
                _ => Vector3.zero
            };

            DrawFloatField(transformType, "X", _currentTabIndex, currentValue.x,
                (newValue) => {
                    Vector3 newVec = new Vector3(newValue, currentValue.y, currentValue.z);
                    applyChange(target, newVec);
                    UpdateInputStateValue(transformType, "X", newValue);
                }, step);

            DrawFloatField(transformType, "Y", _currentTabIndex, currentValue.y,
                (newValue) => {
                    Vector3 newVec = new Vector3(currentValue.x, newValue, currentValue.z);
                    applyChange(target, newVec);
                    UpdateInputStateValue(transformType, "Y", newValue);
                }, step);

            DrawFloatField(transformType, "Z", _currentTabIndex, currentValue.z,
                (newValue) => {
                    Vector3 newVec = new Vector3(currentValue.x, currentValue.y, newValue);
                    applyChange(target, newVec);
                    UpdateInputStateValue(transformType, "Z", newValue);
                }, step);

            GUILayout.EndHorizontal();
        }

        private void UpdateInputStateValue(string transformType, string axis, float value)
        {
            string key = $"{GetTabName(_currentTabIndex)}_{transformType}_{axis}";
            if (_inputStates.ContainsKey(key) && !_inputStates[key].isFocused)
            {
                _inputStates[key].value = value;
                _inputStates[key].text = value.ToString("F4");
            }
        }

        private string GetTabName(int tabIndex)
        {
            return tabIndex == 0 ? "Head" : (tabIndex == 1 ? "Body" : "Face");
        }

        private void DrawFloatField(string transformType, string axisLabel, int tabIndex, float currentValue, System.Action<float> onValueChanged, float step)
        {
            string key = $"{GetTabName(tabIndex)}_{transformType}_{axisLabel}";

            if (!_inputStates.ContainsKey(key))
            {
                _inputStates[key] = new FloatInputState
                {
                    value = currentValue,
                    text = currentValue.ToString("F4"),
                    controlId = _nextControlId++
                };
            }

            var state = _inputStates[key];

            GUILayout.BeginVertical(GUILayout.Width(100));

            GUILayout.Label(axisLabel, GUILayout.Width(100));

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(20)))
            {
                float newValue = currentValue - step;
                onValueChanged(newValue);
                state.value = newValue;
                if (!state.isFocused) state.text = newValue.ToString("F4");
            }

            string displayText = state.isFocused ? state.text : currentValue.ToString("F4");
            GUI.SetNextControlName($"field_{key}");
            string newText = GUILayout.TextField(displayText, GUILayout.Width(50), GUILayout.Height(20));

            if (Event.current.type == EventType.Repaint)
            {
                bool isNowFocused = GUI.GetNameOfFocusedControl() == $"field_{key}";

                if (isNowFocused && !state.isFocused)
                {
                    state.isFocused = true;
                    state.text = currentValue.ToString(CultureInfo.InvariantCulture);
                }
                else if (!isNowFocused && state.isFocused)
                {
                    state.isFocused = false;
                    if (TryParseFloat(state.text, out float parsedValue))
                    {
                        onValueChanged(parsedValue);
                        state.value = parsedValue;
                    }
                }
            }

            if (newText != displayText)
            {
                state.text = newText;

                if (TryParseFloat(newText, out float realTimeValue))
                {
                    onValueChanged(realTimeValue);
                    state.value = realTimeValue;
                }
            }

            if (GUILayout.Button("+", GUILayout.Width(25), GUILayout.Height(20)))
            {
                float newValue = currentValue + step;
                onValueChanged(newValue);
                state.value = newValue;
                if (!state.isFocused) state.text = newValue.ToString("F4");
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private bool TryParseFloat(string input, out float result)
        {
            string normalized = input.Replace(',', '.');
            return float.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        private void LogCurrentSettings()
        {
            string log = "\n╔══════════════════════════════════════════════════════════╗\n";
            log += "║           COSMETIC DEFINITION SETTINGS                       ║\n";
            log += "╚══════════════════════════════════════════════════════════════╝\n\n";
            log += FormatPart("Head", _headObj);
            log += "\n" + FormatPart("Body", _bodyObj);
            log += "\n" + FormatPart("Face", _faceObj);
            log += "\n══════════════════════════════════════════════════════════════";
            Debug.Log(log);
        }

        private string FormatPart(string name, GameObject obj)
        {
            if (obj == null) return $"✗ {name}: Not found\n";

            Vector3 pos = obj.transform.localPosition;
            Vector3 rot = obj.transform.localEulerAngles;
            Vector3 scale = obj.transform.localScale;

            return $"► {name.ToUpper()} ◄\n" +
                   $"  Position: ({FormatFloat(pos.x)}, {FormatFloat(pos.y)}, {FormatFloat(pos.z)})\n" +
                   $"  Rotation: ({FormatFloat(rot.x)}, {FormatFloat(rot.y)}, {FormatFloat(rot.z)})\n" +
                   $"  Scale:    ({FormatFloat(scale.x)}, {FormatFloat(scale.y)}, {FormatFloat(scale.z)})\n";
        }

        private string FormatFloat(float value)
        {
            return value.ToString("F4", CultureInfo.InvariantCulture);
        }
    }
}