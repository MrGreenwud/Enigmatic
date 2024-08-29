using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Teststuff
{

    public static class RectExt
    {
        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        public static Rect ScaleSizeBy(this Rect rect, float scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }

        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            var result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }

        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }

        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
        {
            var result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale.x;
            result.xMax *= scale.x;
            result.yMin *= scale.y;
            result.yMax *= scale.y;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }
    }

    #region PanAndZoom
    [Serializable]
    public class PanAndZoom
    {
        #region Variables

        private Vector2 Offset = Vector2.zero;
        public Vector2 Pan = new Vector2(0, 0);
        public float zoom = 1;
        private Vector2 PanOffset;
        private Matrix4x4 matrix;
        private Matrix4x4 normalMatrix = Matrix4x4.TRS(new Vector3(), Quaternion.identity, Vector3.one);
        private Vector2 mousePos;
        #endregion

        #region Begin End Area

        public void BeginArea(Rect AreaRect, float min, float max, bool resetPan)
        {
            GUI.EndGroup();
            #region Pan
            if (Offset == Vector2.zero && Event.current.rawType == EventType.MouseDown)
                Offset = Event.current.mousePosition;

            if (Event.current.rawType == EventType.MouseDrag && (Event.current.button == 2 || Event.current.alt))
            {
                Pan = Event.current.mousePosition - Offset + PanOffset;
                Event.current.Use();
            }

            if (Event.current.rawType == EventType.MouseUp)
            {
                Offset = Vector2.zero;
                PanOffset = Pan;
            }
            #endregion

            #region zoom
            if (Event.current.type == EventType.ScrollWheel)
            {
                if (mousePos != Event.current.mousePosition)
                    mousePos = Event.current.mousePosition;

                Vector2 delta = Event.current.delta;


                float zoomDelta = -delta.y / 150.0f;
                zoom += zoomDelta * 4;
                zoom = Mathf.Clamp(zoom, min, max);

                Event.current.Use();
            }

            var rect = AreaRect.ScaleSizeBy(1f / zoom, AreaRect.TopLeft());
            rect.y += 21;
            AreaRect = rect;

            Matrix4x4 trs = Matrix4x4.TRS(AreaRect.TopLeft(), Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoom, zoom, zoom));

            // once we begin zooming out , the pan speed of the content zoomed and the unzoomed canvas (if your canvas size wont change ) will pan slower, so we incleae its pan speed by * (1f / zoom)

            GUI.BeginClip(AreaRect, (Pan * (1f / zoom)), Vector2.zero, resetPan);

            GUI.matrix = trs * scale * trs.inverse * GUI.matrix;

            // this is temporarly used as a reference for the window position and scale
            GUI.Box(AreaRect,"I am  the size of the window");

            #endregion
        }


        public void EndArea()
        {
            GUI.matrix = normalMatrix;
            GUI.EndClip();
            GUI.BeginGroup(new Rect(0f, 21, Screen.width, Screen.height));



        }

        #endregion

    }

    #endregion



    //USAGE 



    [Serializable]
    public class TestEditor : EditorWindow
    {

        private PanAndZoom panAndZoom;

        [MenuItem("Tools/TestEditor")]
        static void Init()
        {
            EditorWindow shootEditor = GetWindow<TestEditor>();
            shootEditor.titleContent.text = "TestEditorr";

        }

        public void OnEnable()
        {
            if (panAndZoom == null)
            {

                panAndZoom = new PanAndZoom();
            }
        }


        public void OnGUI()
        {

            panAndZoom.BeginArea(new Rect(0, 0, Screen.width, Screen.height), 0.3f, 1, false);

            GUI.Box(new Rect(300, 200, 100, 40), " i do nothing");
            GUI.Box(new Rect(600, 350, 100, 40), " i do nothing either");

            panAndZoom.EndArea();

        }

    }
}