using UnityEngine;
using UnityEditor;

namespace Galacron.Formations
{
    [CustomEditor(typeof(FormationConfig))]
    public class FormationConfigEditor : UnityEditor.Editor
    {
        private FormationGridTool gridTool;
        private FormationSymmetryTool symmetryTool;
        private FormationSlotEditor slotEditor;
        private FormationPreviewTool previewTool;
        
        // Template-related fields
        private bool showTemplateSection = true;
        private string newTemplateName = "";
        private Vector2 templateScrollPosition;
        private FormationTemplate selectedTemplate;

        // Serialized properties
        private SerializedProperty slotPositionsProperty;
        private SerializedProperty spacingProperty;
        private SerializedProperty arrivalDelayBetweenMembersProperty;
        private SerializedProperty idleAmplitudeProperty;
        private SerializedProperty idleFrequencyProperty;
        private SerializedProperty moveSpeedProperty;
        private SerializedProperty rotationSpeedProperty;
        private SerializedProperty attackCooldownProperty;
        private SerializedProperty maxSimultaneousAttackersProperty;
        private SerializedProperty attackProbabilityProperty;
        private SerializedProperty entryPathProperty;
        private SerializedProperty attackPathProperty;
        private SerializedProperty returnPathProperty;

        private bool showBasicSettings = true;
        private bool showMovementSettings = true;
        private bool showAttackSettings = true;
        private bool showPathSettings = true;

        private void OnEnable()
        {
            // Initialize properties
            slotPositionsProperty = serializedObject.FindProperty("slotPositions");
            spacingProperty = serializedObject.FindProperty("spacing");
            arrivalDelayBetweenMembersProperty = serializedObject.FindProperty("arrivalDelayBetweenMembers");
            idleAmplitudeProperty = serializedObject.FindProperty("idleAmplitude");
            idleFrequencyProperty = serializedObject.FindProperty("idleFrequency");
            moveSpeedProperty = serializedObject.FindProperty("moveSpeed");
            rotationSpeedProperty = serializedObject.FindProperty("rotationSpeed");
            attackCooldownProperty = serializedObject.FindProperty("attackCooldown");
            maxSimultaneousAttackersProperty = serializedObject.FindProperty("maxSimultaneousAttackers");
            attackProbabilityProperty = serializedObject.FindProperty("attackProbability");
            entryPathProperty = serializedObject.FindProperty("entryPath");
            attackPathProperty = serializedObject.FindProperty("attackPath");
            returnPathProperty = serializedObject.FindProperty("returnPath");

            // Initialize tools
            gridTool = new FormationGridTool();
            symmetryTool = new FormationSymmetryTool();
            slotEditor = new FormationSlotEditor(slotPositionsProperty);
            previewTool = new FormationPreviewTool();

            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            gridTool.OnDisable();
            symmetryTool.OnDisable();
            slotEditor.OnDisable();
            previewTool.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // Draw template section
            DrawTemplateSection();
            EditorGUILayout.Space(10);

            // Grid settings section
            gridTool.OnInspectorGUI();
            EditorGUILayout.Space(10);

            // Preview section
            previewTool.OnInspectorGUI(slotPositionsProperty, spacingProperty.floatValue);
            EditorGUILayout.Space(10);

            // Symmetry tools section
            symmetryTool.OnInspectorGUI(slotPositionsProperty);
            EditorGUILayout.Space(10);

            // Slot editing section
            slotEditor.OnInspectorGUI();
            EditorGUILayout.Space(10);

            // Formation settings sections
            DrawFormationSettings();

            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawTemplateSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            showTemplateSection = EditorGUILayout.Foldout(showTemplateSection, "Formation Templates", true);

            if (showTemplateSection)
            {
                EditorGUI.indentLevel++;

                // Template list
                var templates = FormationTemplateManager.GetAllTemplates();
                templateScrollPosition = EditorGUILayout.BeginScrollView(templateScrollPosition, GUILayout.Height(100));
                
                foreach (var template in templates)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    bool isSelected = selectedTemplate == template;
                    bool newSelected = EditorGUILayout.ToggleLeft(template.templateName, isSelected);
                    
                    if (newSelected != isSelected)
                    {
                        selectedTemplate = newSelected ? template : null;
                    }

                    if (GUILayout.Button("Apply", GUILayout.Width(60)))
                    {
                        if (EditorUtility.DisplayDialog("Apply Template",
                            "Are you sure you want to apply this template? This will override current formation settings.",
                            "Apply", "Cancel"))
                        {
                            template.ApplyToConfig(target as FormationConfig);
                            serializedObject.Update();
                        }
                    }

                    if (GUILayout.Button("Delete", GUILayout.Width(60)))
                    {
                        if (EditorUtility.DisplayDialog("Delete Template",
                            "Are you sure you want to delete this template?",
                            "Delete", "Cancel"))
                        {
                            FormationTemplateManager.DeleteTemplate(template);
                            if (selectedTemplate == template)
                            {
                                selectedTemplate = null;
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndScrollView();

                // Template details
                if (selectedTemplate != null)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Template Details:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Slots:", selectedTemplate.slotPositions?.Length.ToString() ?? "0");
                    EditorGUILayout.LabelField("Spacing:", selectedTemplate.spacing.ToString());
                    EditorGUILayout.LabelField("Arrival Delay:", selectedTemplate.arrivalDelayBetweenMembers.ToString());
                    EditorGUI.indentLevel--;
                }

                // Create new template
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                newTemplateName = EditorGUILayout.TextField("New Template Name", newTemplateName);
                
                EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(newTemplateName));
                if (GUILayout.Button("Create", GUILayout.Width(60)))
                {
                    var template = FormationTemplateManager.CreateTemplate(newTemplateName, target as FormationConfig);
                    selectedTemplate = template;
                    newTemplateName = "";
                }
                EditorGUI.EndDisabledGroup();
                
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawFormationSettings()
        {
            // Basic Settings
            showBasicSettings = EditorGUILayout.Foldout(showBasicSettings, "Basic Settings", true);
            if (showBasicSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spacingProperty);
                EditorGUILayout.PropertyField(arrivalDelayBetweenMembersProperty);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Movement Settings
            showMovementSettings = EditorGUILayout.Foldout(showMovementSettings, "Movement Settings", true);
            if (showMovementSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(idleAmplitudeProperty);
                EditorGUILayout.PropertyField(idleFrequencyProperty);
                EditorGUILayout.PropertyField(moveSpeedProperty);
                EditorGUILayout.PropertyField(rotationSpeedProperty);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Attack Settings
            showAttackSettings = EditorGUILayout.Foldout(showAttackSettings, "Attack Settings", true);
            if (showAttackSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(attackCooldownProperty);
                EditorGUILayout.PropertyField(maxSimultaneousAttackersProperty);
                EditorGUILayout.PropertyField(attackProbabilityProperty);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Path Settings
            showPathSettings = EditorGUILayout.Foldout(showPathSettings, "Path Settings", true);
            if (showPathSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(entryPathProperty, new GUIContent("Entry Path"), true);
                EditorGUILayout.PropertyField(attackPathProperty, new GUIContent("Attack Path"), true);
                EditorGUILayout.PropertyField(returnPathProperty, new GUIContent("Return Path"), true);
                EditorGUI.indentLevel--;
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (target == null) return;

            // Draw grid if enabled
            gridTool.OnSceneGUI(sceneView);

            // Draw symmetry guides if in symmetry mode
            symmetryTool.OnSceneGUI();

            // Draw slot handles if in edit mode
            slotEditor.OnSceneGUI(gridTool, spacingProperty.floatValue);

            // Consume input to prevent default tool behavior when editing
            if (slotEditor.IsEditing)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
        }
    }
}