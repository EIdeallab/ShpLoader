using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Assets
{
    public class ShapeRendererUI : EditorWindow
    {
        private static ShapeRendererUI window;
        
        private Rect SelectBox;
        private Rect RenderBox;

        private static string shpFilePath;
        private static string dbfFilePath;

        private string ShpFileName { get { return Path.GetFileName(shpFilePath); } }
        private string DbfFileName { get { return Path.GetFileName(dbfFilePath); } }

        private static IShpFile shapeFile;
        private static DbfFile dbfFile;

        private static int shpFileCode;
        private static int shpFileLength;
        private static int shpFileVersion;
        private static string shpFileType;
        private static string dbfFileVersion;
        private static int dbfFileDate;
        private static int dbfFileRecordCnt;

        private static Color shapeColor;

        [MenuItem("ShapeLoader/Loader")]
        private static void Init()
        {
            window = GetWindow<ShapeRendererUI>();
            window.maxSize = window.minSize = new Vector2(300, 400);
            window.Show();;

            shpFilePath = "";
            dbfFilePath = "";

            shapeColor = Color.white;
        }

        private void OnGUI()
        {
            // fixed window size
            GUILayout.ExpandHeight(false);
            GUILayout.ExpandWidth(false);
            
            SelectBox = new Rect(3, 0, position.width - 6, EditorGUIUtility.singleLineHeight * 4);
            RenderBox = new Rect(3, EditorGUIUtility.singleLineHeight * 4, position.width - 6, EditorGUIUtility.singleLineHeight * 16);

            #region Select Box
            GUILayout.BeginArea(SelectBox);
            GUILayout.Label("- Select object shape file : ", GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Shape", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.fieldWidth * 2)))
            {
                shpFilePath = EditorUtility.OpenFilePanel("Shape Loader(shx or shp)", "", "shp,shx");
            }
            EditorGUILayout.SelectableLabel(ShpFileName, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Dbf", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.fieldWidth * 2)))
            {
                dbfFilePath = EditorUtility.OpenFilePanel("Data file Loader", "", "dbf");
            }
            EditorGUILayout.SelectableLabel(DbfFileName, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            #endregion

            #region Render Box
            // If Shp file is not selected, shape can't be loaded
            GUILayout.BeginArea(RenderBox);

            // Check file path
            if (shpFilePath.Length == 0)
                GUI.enabled = false;
            else
                GUI.enabled = true;

            if (GUILayout.Button("Load Data"))
            {
                shapeFile = LoadFiles(shpFilePath) as ShpFile;
                dbfFile = LoadFiles(dbfFilePath) as DbfFile;
            }

            // Check File header Info
            shpFileCode = (shapeFile != null) ? shapeFile.FileCode : 0;
            shpFileLength = (shapeFile != null) ? shapeFile.FileLength : 0;
            shpFileVersion = (shapeFile != null) ? shapeFile.FileVersion : 0;
            shpFileType = (shapeFile != null) ? shapeFile.ShpType.ToString() : "";
            dbfFileVersion = (dbfFile != null) ? dbfFile.Version.ToString() : "";
            dbfFileDate = (dbfFile != null) ? dbfFile.UpdateDate : 0;
            dbfFileRecordCnt = (dbfFile != null) ? dbfFile.NumberOfRecords : 0;

            // Shp File Header Info Field
            GUILayout.Label("- Shp file Description : ", GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.IntField("File Code", shpFileCode, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.IntField("File Length", shpFileLength, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.IntField("File Version", shpFileVersion, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Shape Type", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            EditorGUILayout.SelectableLabel(shpFileType, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.EndHorizontal();

            // Dbf File Header Info Field
            GUILayout.Label("- Dbf file Description : ", GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.LabelField("Dbf Version", GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            EditorGUILayout.SelectableLabel(dbfFileVersion, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.IntField("Dbf Date", dbfFileDate, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.IntField("Record Count", dbfFileRecordCnt, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            

            // Check file loaded correctly  
            if (shapeFile == null)
                GUI.enabled = false;
            else
                GUI.enabled = true;
            

            // Render Field
            GUILayout.Label("- Render Option : ", GUILayout.Height(EditorGUIUtility.singleLineHeight));

            // Render Data
            shapeColor = EditorGUILayout.ColorField("Color", shapeColor, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (GUILayout.Button("Render Data"))
            {
                RenderFiles(shapeColor);
            }
            GUILayout.EndArea();
            #endregion
        }

        private IFile LoadFiles(string path)
        {
            IFile file;
            try
            {
                string fileExt = Path.GetExtension(path);
                file = FileFactory.CreateInstance(path);
                file.Load();
                return file;
            }
            catch (Exception e)
            {
                if(path.Length == 0)
                {
                    Debug.Log("Path is empty.");
                    return null;
                }
                Debug.Log(e);
                return null;
            } 
        }

        private void RenderFiles(Color color)
        {
            try
            {
                ((IRenderable)shapeFile).Render(color);
            }
            catch (Exception e)
            {
                //Debug.Log(e);
            }
        }
    }
}
