using UnityEditor;
using UnityEngine;

namespace Game.Player
{
    [CustomEditor(typeof(PlayerController))]
    public class PlayerEditor : Editor
    {
        private PlayerController _player;

        private void OnEnable()
        {
            _player = (PlayerController)target;
            _player.OnComponentChangedEnabled += _ => Repaint();
        }
        private void OnDisable()
        {
            if(_player != null)
                _player.OnComponentChangedEnabled -= _ => Repaint();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
                return;

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            if (_player.Oxygen != null)
            {
                DrawOxygenButton();
            }
            if (_player.Health != null)
            {
                DrawHealthButton();
            }
            if(_player.Movement != null)
            {
                DrawMovementButton();
            }

            GUILayout.EndHorizontal();
        }

        private void DrawMovementButton()
        {
            string movementEnabledText = _player.Movement.Enabled ? "Disable Movement" : "Enable Movement";
            GUIStyle movementBtnStyle = new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = _player.Movement.Enabled ? Color.green : Color.red }
            };
            if (GUILayout.Button(movementEnabledText, movementBtnStyle))
            {
                if (_player.Movement.Enabled)
                {
                    _player.Movement.Disable();
                }
                else
                {
                    _player.Movement.Enable();
                }
            }
        }

        private void DrawHealthButton()
        {
            string healthEnabledText = _player.Health.Enabled ? "Disable Health" : "Enable Health";
            GUIStyle healthBtnStyle = new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = _player.Health.Enabled ? Color.green : Color.red }
            };
            if (GUILayout.Button(healthEnabledText, healthBtnStyle))
            {
                if (_player.Health.Enabled)
                {
                    _player.Health.Disable();
                }
                else
                {
                    _player.Health.Enable();
                }
            }
        }

        private void DrawOxygenButton()
        {
            string oxygenEnabledText = _player.Oxygen.Enabled ? "Disable Oxygen" : "Enable Oxygen";
            GUIStyle oxygenBtnStyle = new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = _player.Oxygen.Enabled ? Color.green : Color.red }
            };

            if (GUILayout.Button(oxygenEnabledText, oxygenBtnStyle))
            {
                if (_player.Oxygen.Enabled)
                {
                    _player.Oxygen.Disable();
                }
                else
                {
                    _player.Oxygen.Enable();
                }
            }
        }
    }
}