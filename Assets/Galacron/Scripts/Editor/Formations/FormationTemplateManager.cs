using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Galacron.Formations
{
    public class FormationTemplateManager
    {
        private const string TEMPLATE_PATH = "Assets/FormationTemplates";
        private const string TEMPLATE_EXTENSION = ".asset";
        
        public static List<FormationTemplate> GetAllTemplates()
        {
            var templates = new List<FormationTemplate>();
            var guids = AssetDatabase.FindAssets("t:FormationTemplate");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var template = AssetDatabase.LoadAssetAtPath<FormationTemplate>(path);
                if (template != null)
                {
                    templates.Add(template);
                }
            }
            
            return templates;
        }

        public static FormationTemplate CreateTemplate(string name, FormationConfig config)
        {
            if (!System.IO.Directory.Exists(TEMPLATE_PATH))
            {
                System.IO.Directory.CreateDirectory(TEMPLATE_PATH);
            }

            var template = ScriptableObject.CreateInstance<FormationTemplate>();
            template.templateName = name;
            template.CopyFromConfig(config);

            var path = $"{TEMPLATE_PATH}/{name}{TEMPLATE_EXTENSION}";
            AssetDatabase.CreateAsset(template, path);
            AssetDatabase.SaveAssets();
            
            return template;
        }

        public static void DeleteTemplate(FormationTemplate template)
        {
            var path = AssetDatabase.GetAssetPath(template);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
        }
    }
}