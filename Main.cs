/*
 * Autorius: Giedrius Kristinaitis
 * 8 Laboratorinis darbas
 */

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace L8_Kristinaitis {

    public class Main {

        // command method to draw a net
        [CommandMethod("Tinklelis")]
        public void Tinklelis() {
            Database db = HostApplicationServices.WorkingDatabase;
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
            Editor editor = doc.Editor;

            Point3d corner = GetPoint3d(editor, "Pasirinkite pradžios tašką: ");
            double width = GetDoubleValue(editor, "Įveskite ilgį: ", false, false);
            double height = GetDoubleValue(editor, "Įveskite plotį: ", false, false);

            // set correct zoom level
            editor.Command("ZOOM", new Point2d(corner.X - Math.Max(width, height) * 0.2, corner.Y - Math.Max(width, height) * 0.2), new Point2d(corner.X + Math.Max(width, height) * 1.2, corner.Y + Math.Max(width, height) * 1.2));

            DrawNet(editor, corner, width, height);
        }

        // method that draws a net
        private void DrawNet(Editor editor, Point3d corner, double width, double height) {
            double horizontalDistance = width / 4;
            double verticalDistance = height / 9;

            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 10; j++) {
                    // draw circle
                    Point3d circleCenter = new Point3d(corner.X + (horizontalDistance * i) + (horizontalDistance / 2 * j), corner.Y + (verticalDistance * j), 0);
                    editor.Command("CIRCLE", circleCenter, Math.Min(width, height) / 15);

                    // draw lines
                    if (i < 4) {
                        editor.Command("LINE", circleCenter, new Point3d(circleCenter.X + horizontalDistance, circleCenter.Y, 0), "");
                    }

                    if (j < 9) {
                        editor.Command("LINE", circleCenter, new Point3d(circleCenter.X + horizontalDistance / 2, circleCenter.Y + verticalDistance, 0), "");
                    }
                }
            }
        }

        // command method to draw a rectangle
        [CommandMethod("Keturkampis")]
        public void Keturkampis() {
            Database db = HostApplicationServices.WorkingDatabase;
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
            Editor editor = doc.Editor;

            Point3d center = GetPoint3d(editor, "Pasirinkite centro tašką: ");
            double width = GetDoubleValue(editor, "Įveskite ilgį: ", false, false);
            double height = GetDoubleValue(editor, "Įveskite plotį: ", false, false);

            // set correct zoom level
            editor.Command("ZOOM", new Point2d(center.X - Math.Max(width, height) * 1.2, center.Y - Math.Max(width, height) * 1.2), new Point2d(center.X + Math.Max(width, height) * 1.2, center.Y + Math.Max(width, height) * 1.2));

            DrawRectangle(editor, center, width, height);
        }

        // method that draws a rectangle
        private void DrawRectangle(Editor editor, Point3d center, double width, double height) {
            Point3d topLeft = new Point3d(center.X - width / 2, center.Y + height / 2, 0);
            Point3d topRight = new Point3d(center.X + width / 2, center.Y + height / 2, 0);
            Point3d bottomRight = new Point3d(center.X + width / 2, center.Y - height / 2, 0);
            Point3d bottomLeft = new Point3d(center.X - width / 2, center.Y - height / 2, 0);

            // draw rectangle
            editor.Command("LINE", topLeft, topRight, "");
            editor.Command("LINE", topRight, bottomRight, "");
            editor.Command("LINE", bottomRight, bottomLeft, "");
            editor.Command("LINE", bottomLeft, topLeft, "");

            // draw inner lines
            DrawInnerLines(editor, topLeft, 1, 4, center, width, height);
            DrawInnerLines(editor, topRight, -1, 4, center, width, height);

            // draw sides
            editor.Command("LINE", topLeft, new Point3d(topLeft.X - width / 10, topLeft.Y + height / 10, 0), "");
            editor.Command("LINE", new Point3d(topLeft.X - width / 10, topLeft.Y + height / 10, 0), bottomLeft, "");
            editor.Command("-HATCH", new Point2d(topLeft.X - width / 20, topLeft.Y), "p", "ANSI31", "", "", "");

            editor.Command("LINE", topRight, new Point3d(topRight.X + width / 10, topRight.Y + height / 10, 0), "");
            editor.Command("LINE", new Point3d(topRight.X + width / 10, topRight.Y + height / 10, 0), bottomRight, "");
            editor.Command("-HATCH", new Point2d(topRight.X + width / 20, topRight.Y), "p", "ANSI31", "", "", "");
        }

        // method that draws lines inside a rectangle
        private void DrawInnerLines(Editor editor, Point3d corner, int direction, int lineCount, Point3d center, double width, double height) {
            for (int i = 0; i < lineCount; i++) {
                Point3d lineEnd = new Point3d(center.X + (direction * (width / 2)), center.Y - (height / 2 - ((height / 2 / (lineCount - 1)) * i)), 0);
                editor.Command("LINE", corner, lineEnd, "");
            }
        }

        // command method to draw a wheel
        [CommandMethod("Ratas")]
        public void Ratas() {
            Database db = HostApplicationServices.WorkingDatabase;
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
            Editor editor = doc.Editor;

            Point3d center = GetPoint3d(editor, "Pasirinkite centro tašką: ");
            double radius = GetDoubleValue(editor, "Įveskite spindulį: ", false, false);

            // set correct zoom level
            editor.Command("ZOOM", new Point2d(center.X - radius * 1.2, center.Y - radius * 1.2), new Point2d(center.X + radius * 1.2, center.Y + radius * 1.2));

            DrawWheel(editor, center, radius);
        }

        // method that draws a wheel
        private void DrawWheel(Editor editor, Point3d center, double radius) {
            double thickness = radius * 0.2;

            // draw big circles
            editor.Command("CIRCLE", center, radius);
            editor.Command("CIRCLE", center, radius - thickness);
            editor.Command("-HATCH", new Point2d(center.X + radius - thickness * 0.5, center.Y), "p", "s", "", "");

            editor.Command("CIRCLE", center, thickness * 1.5);
            editor.Command("CIRCLE", center, thickness * 0.9);
            editor.Command("-HATCH", new Point2d(center.X + thickness * 1.2, center.Y), "p", "s", "", "");

            // draw small circles
            double distance = thickness * 0.85;
            double diagonalDistance = Math.Sqrt(Math.Pow(thickness * 0.85, 2) / 2);

            for (int i = 0; i < 5; i++) {
                if (i == 3) {
                    continue;
                } else if (i == 4) {
                    distance = thickness * 0.9125;
                    diagonalDistance = Math.Sqrt(Math.Pow(thickness * 0.9125, 2) / 2) - 0.0275;
                }

                Point3d[] circleCenterPoints = new Point3d[] {
                    new Point3d(center.X, center.Y + thickness * 1.85 + (distance * i), 0),
                    new Point3d(center.X + thickness * 1.35 + (diagonalDistance * i), center.Y + thickness * 1.35 + (diagonalDistance * i), 0),
                    new Point3d(center.X + thickness * 1.85 + (distance * i), center.Y, 0),
                    new Point3d(center.X + thickness * 1.35 + (diagonalDistance * i), center.Y - thickness * 1.35 - (diagonalDistance * i), 0),
                    new Point3d(center.X, center.Y - thickness * 1.85 - (distance * i), 0),
                    new Point3d(center.X - thickness * 1.35 - (diagonalDistance * i), center.Y - thickness * 1.35 - (diagonalDistance * i), 0),
                    new Point3d(center.X - thickness * 1.85 - (distance * i), center.Y, 0),
                    new Point3d(center.X - thickness * 1.35 - (diagonalDistance * i), center.Y + thickness * 1.35 + (diagonalDistance * i), 0)
                };

                foreach (Point3d circleCenter in circleCenterPoints) {
                    editor.Command("CIRCLE", circleCenter, thickness * 0.5);
                    editor.Command("-HATCH", circleCenter, "p", "s", "", "");
                    editor.WriteMessage(string.Format("\n{0} {1}", circleCenter.X, circleCenter.Y));
                }
            }
        }

        // command method to draw a face
        [CommandMethod("Veidas")]
        public void Apskritimas() {
            Database db = HostApplicationServices.WorkingDatabase;
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.GetDocument(db);
            Editor editor = doc.Editor;

            Point3d center = GetPoint3d(editor, "Pasirinkite centro tašką: ");
            double radius = GetDoubleValue(editor, "Įveskite spindulį: ", false, false);

            // set correct zoom level
            editor.Command("ZOOM", new Point2d(center.X - radius * 1.2, center.Y - radius * 1.2), new Point2d(center.X + radius * 1.2, center.Y + radius * 1.2));

            DrawFace(editor, center, radius);
        }

        // method that draws a face
        private void DrawFace(Editor editor, Point3d center, double radius) {
            double thickness = radius * 0.1;

            // draw outline
            editor.Command("CIRCLE", center, radius);
            editor.Command("CIRCLE", center, radius - thickness);
            editor.Command("-HATCH", new Point2d(center.X + radius - thickness * 0.5, center.Y), "p", "s", "", "");

            // draw eyes
            Point3d leftEyeCenter = new Point3d(center.X - thickness * 3, center.Y + thickness * 3, 0);
            Point3d rightEyeCenter = new Point3d(center.X + thickness * 3, center.Y + thickness * 3, 0);

            editor.Command("CIRCLE", leftEyeCenter, thickness * 1.2);
            editor.Command("-HATCH", new Point2d(leftEyeCenter.X, leftEyeCenter.Y), "p", "s", "", "");
            editor.Command("CIRCLE", rightEyeCenter, thickness * 1.2);
            editor.Command("-HATCH", new Point2d(rightEyeCenter.X, rightEyeCenter.Y), "p", "s", "", "");

            // draw mouth
            Point3d topLeft = new Point3d(center.X - thickness * 5, center.Y - thickness * 3, 0);
            Point3d topRight = new Point3d(center.X + thickness * 5, center.Y - thickness * 3, 0);

            editor.Command("ARC", topLeft, topRight, new Point2d(topRight.X, topRight.Y + thickness * 0.01));
            editor.Command("ARC", new Point2d(topLeft.X + thickness, topLeft.Y), new Point2d(topRight.X - thickness, topRight.Y), new Point2d(topRight.X - thickness, topRight.Y + thickness * 0.01));
            editor.Command("LINE", topLeft, new Point2d(topLeft.X + thickness, topLeft.Y), "");
            editor.Command("LINE", new Point2d(topRight.X - thickness, topRight.Y + thickness * 0.01), new Point2d(topRight.X, topRight.Y + thickness * 0.01), "");
            editor.Command("-HATCH", new Point2d(topLeft.X + thickness * 0.5, topLeft.Y - thickness * 0.2), "p", "s", "", "");

            editor.Command("-COLOR", "t", "255, 255, 0", "");
            editor.Command("-HATCH", center, "p", "s", "", "");
            editor.Command("-COLOR", "BYLAYER", "");
        }

        // method to get a double value
        private double GetDoubleValue(Editor editor, string message, bool negative, bool zero) {
            PromptDoubleOptions options = new PromptDoubleOptions("\n" + message);
            options.AllowNone = false;
            options.AllowZero = zero;
            options.AllowNegative = negative;

            return editor.GetDouble(options).Value;
        }

        // method to get a 3d point
        private Point3d GetPoint3d(Editor editor, string message) {
            PromptPointOptions options = new PromptPointOptions("\n" + message);
            options.AllowNone = false;

            return editor.GetPoint(options).Value;
        }
    }
}
