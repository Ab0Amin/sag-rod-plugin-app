using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using t3d = Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Solid;
using System.Windows.Forms;
using System.Linq;

namespace sag_rod_plugin_app
{
    public partial class Form1 : Form
    {
        Model myModel = new Model();
        public Form1()
        {
            InitializeComponent();
        }
        double web_1_width = 0;
        double web_2_width = 0;
       

        // rod
        string rod_profile = "ROD16";
        string rod_material = "A36";
        double spacing = 70;
        double HzOffset_start = 150;
        double HzOffset_end = 100;
        double depth = 50;

        // hole 
        double edge_1 = 35;
        double edge_2 = 35;


        // plate
        double plateThik = 4;
        string plateMaterial = "A36";
        double plateWidth = 140;

        double plateLength = 70;
        string plateProfile = null;

        Position.RotationEnum plateRotation;



        private void button1_Click(object sender, EventArgs e)
        {
            Picker inputs = new Picker();
            ModelObjectEnumerator rafter_1 = inputs.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, "Pick webs of first Rafter");
            ModelObjectEnumerator rafter_2 = inputs.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, "Pick webs of second Rafter");
            t3d.Point poinst_1 = inputs.PickPoint() as t3d.Point;
            t3d.Point poinst_2 = inputs.PickPoint() as t3d.Point;



            List<Beam> rafter_1_web = new List<Beam>();
            List<Beam> rafter_2_web = new List<Beam>();
            while (rafter_1.MoveNext())
            {
                Beam web1 = rafter_1.Current as Beam;
                if (web1 != null)
                {
                    rafter_1_web.Add(web1);
                }
            }

            while (rafter_2.MoveNext())
            {
                Beam web2 = rafter_2.Current as Beam;
                if (web2 != null)
                {
                    rafter_2_web.Add(web2);
                }
            }


            Beam web_1 = rafter_1_web[0];
            Beam web_2 = rafter_2_web[0];

            CoordinateSystem coordinate = web_1.GetCoordinateSystem();
            Vector directionX = new Vector(coordinate.AxisX);
            directionX.Normalize();
            Vector directionY = new Vector(coordinate.AxisY);
            directionY.Normalize();








            Point orgin_1 = web_1.GetCenterLine(false)[0] as t3d.Point;
            web_1.GetReportProperty("HEIGHT", ref web_1_width);
            orgin_1 = orgin_1 - (web_1_width / 2 - depth) * -1 * directionY;

            plateRotation = web_1.Position.Rotation;

            Point orgin_2 = web_2.GetCenterLine(false)[0] as t3d.Point;
            web_2.GetReportProperty("HEIGHT", ref web_2_width);
            orgin_2 = orgin_2 - (web_2_width / 2 - depth) * -1 * directionY;


            GeometricPlane HZplane1 = new GeometricPlane(orgin_1, directionX, directionX.Cross(directionY));
            GeometricPlane vlPlane1 = new GeometricPlane(orgin_1, directionX, (directionY));
            GeometricPlane vlPlane2 = new GeometricPlane(orgin_2, directionX, (directionY));


            t3d.Point projected_point_1 = Projection.PointToPlane(poinst_1, HZplane1);
            t3d.Point projected_point_2 = Projection.PointToPlane(poinst_2, HZplane1);

            projected_point_1 = projected_point_1 + directionX * HzOffset_start;
            projected_point_2 = projected_point_2 - directionX * HzOffset_end;

            t3d.Point rod_1_point1 = Projection.PointToPlane(projected_point_1, vlPlane1);
            t3d.Point rod_2_point1 = Projection.PointToPlane(projected_point_2, vlPlane1);

            t3d.Point rod_2_point2 = Projection.PointToPlane(projected_point_1, vlPlane2);
            t3d.Point rod_1_point2 = Projection.PointToPlane(projected_point_2, vlPlane2);

            Beam rod1 = insertRod(rod_1_point1, rod_1_point2);
            Beam rod2 = insertRod(rod_2_point1, rod_2_point2);

            t3d.Point Plate_1_Point1 = rod_1_point1 - plateWidth / 2 * directionX;

            t3d.Point Plate_2_Point1 = rod_1_point2 - plateWidth / 2 * directionX;

            t3d.Point Plate_3_Point1 = rod_2_point1 - plateWidth / 2 * directionX;

            t3d.Point Plate_4_Point1 = rod_2_point2 - plateWidth / 2 * directionX;

            if (cb_sinleordouble.SelectedIndex == 0)
            {
                spacing = 0;
            }
            else
            {
                Beam rod3 = insertRod(rod_1_point1 - spacing * directionY, rod_1_point2 - spacing * directionY);
                Beam rod4 = insertRod(rod_2_point1 - spacing * directionY, rod_2_point2 - spacing * directionY);



                Plate_1_Point1 = Plate_1_Point1 - spacing / 2 * directionY;

                Plate_2_Point1 = Plate_2_Point1 - spacing / 2 * directionY;

                Plate_3_Point1 = Plate_3_Point1 - spacing / 2 * directionY;

                Plate_4_Point1 = Plate_4_Point1 - spacing / 2 * directionY;


            }

            plateLength = edge_1 + edge_2 + spacing;
            plateProfile = "PL" + plateThik + "*" + plateLength;

            t3d.Point Plate_1_Point2 = Plate_1_Point1 + plateWidth * directionX;

            t3d.Point Plate_2_Point2 = Plate_2_Point1 + plateWidth * directionX;

            t3d.Point Plate_3_Point2 = Plate_3_Point1 + plateWidth * directionX;

            t3d.Point Plate_4_Point2 = Plate_4_Point1 + plateWidth * directionX;

            Beam plate1 = insertPlate(Plate_1_Point1, Plate_1_Point2);
            plate1.Class = "2";
            plate1.Modify();
            Beam plate2 = insertPlate(Plate_2_Point1, Plate_2_Point2);
            Beam plate3 = insertPlate(Plate_3_Point1, Plate_3_Point2);
            Beam plate4 = insertPlate(Plate_4_Point1, Plate_4_Point2);

            Beam holeWeb_1 = null, holeWeb_2 = null, holeWeb_3 = null, holeWeb_4 = null;
            LineSegment line_1 = new LineSegment(rod_1_point1, rod_1_point2);
            LineSegment line_2 = new LineSegment(rod_2_point1, rod_2_point2);

            for (int i = 0; i < rafter_1_web.Count; i++)
            {
                Beam current = rafter_1_web[i];
                Solid solid = current.GetSolid();
                if (solid.Intersect(line_1)!= null)
                {
                    holeWeb_1 = current;
                }
                if (solid.Intersect(line_2) != null)
                {
                    holeWeb_2 = current;
                }
            }
            for (int i = 0; i < rafter_2_web.Count; i++)
            {
                Beam current = rafter_2_web[i];
                Solid solid = current.GetSolid();
                if (solid.Intersect(line_2) != null)
                {
                    holeWeb_3 = current;
                }
                if (solid.Intersect(line_1) != null)
                {
                    holeWeb_4 = current;
                }
            }
            crateBolt(Plate_1_Point1, Plate_1_Point2, plate1, 20 / 2, 50, 2, "4.6CSK", 0, 0,"0",spacing.ToString(), BoltGroup.BoltTypeEnum.BOLT_TYPE_WORKSHOP, true, true, 50, 500);


            myModel.CommitChanges();
        }
        public Beam insertRod(Point start_point, Point end_point)
        {
            Beam beam = new Beam();
            beam.StartPoint = start_point;
            beam.EndPoint = end_point;
            beam.Profile.ProfileString =rod_profile ;
            beam.Material.MaterialString = rod_material;
            beam.StartPointOffset.Dx =-150 ;
            beam.EndPointOffset.Dx = 150;
            beam.PartNumber.Prefix="W";
            beam.PartNumber.StartNumber = 101;
            beam.AssemblyNumber.StartNumber = 4001;
            beam.AssemblyNumber.Prefix = "";
            beam.Position.Depth = Position.DepthEnum.MIDDLE;
            beam.Position.Plane = Position.PlaneEnum.MIDDLE;
            beam.Position.Rotation = Position.RotationEnum.TOP;
            beam.Insert();
            return beam;
        }

        public Beam insertPlate(Point start_point, Point end_point)
        {
            Beam beam = new Beam();
            beam.StartPoint = start_point;
            beam.EndPoint = end_point;
            beam.Profile.ProfileString = plateProfile;
            beam.Material.MaterialString = plateMaterial;

            beam.PartNumber.Prefix = "W";
            beam.PartNumber.StartNumber = 101;
            beam.AssemblyNumber.StartNumber = 4001;
            beam.AssemblyNumber.Prefix = "";
            beam.Position.Depth = Position.DepthEnum.MIDDLE;
            beam.Position.Plane = Position.PlaneEnum.MIDDLE;
            beam.Position.Rotation = plateRotation;
            beam.Insert();
            return beam;
        }

        public Beam check_with_beam(Point start_point, Point end_point)
        {
            Beam beam = new Beam();
            beam.StartPoint = start_point;
            beam.EndPoint = end_point;
            beam.Profile.ProfileString = "ROD100";
            beam.Position.Depth = Position.DepthEnum.FRONT;
            beam.Position.Plane = Position.PlaneEnum.RIGHT;
            beam.Position.Rotation = Position.RotationEnum.TOP;
            beam.Insert();
            return beam;
        }
        public BoltArray crateBolt (Point platePoint1, Point plateMidPoint, Part mainPart, double dx, double boltSize,
               double tolerance, string boltStandard, int no_bolt_x, int no_bolt_y, string spacing_x, string spacing_y,
                 BoltArray.BoltTypeEnum boltType, bool sloyInMainPart, bool sloyInSecnPart, double slot_X, double slot_y)
        {

            BoltArray boltArray = new BoltArray();
            boltArray.FirstPosition = plateMidPoint;
            boltArray.SecondPosition = platePoint1;
      
            boltArray.PartToBoltTo = mainPart;
            boltArray.StartPointOffset.Dx = dx;

            boltArray.BoltSize = boltSize;
            boltArray.Tolerance = tolerance;
            boltArray.BoltStandard = boltStandard;
            boltArray.Position.Rotation = plateRotation;

            boltArray.BoltType = boltType;
            boltArray.Hole1 = sloyInMainPart;
            boltArray.Hole2 = sloyInSecnPart;
            boltArray.SlottedHoleX = slot_X;
            boltArray.SlottedHoleY = slot_y;
           
            return boltArray;
        }
    }
}
