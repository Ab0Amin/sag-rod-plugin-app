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
      
       

        // rod
        string rod_profile = "ROD16";
        string rod_material = "A36";
        string rod_perfix = "";
        int rod_startNO = 4001;
        string rod_name = "HORBRACE";
       
        double spacing = 200;
        double HzOffset_start = 150;
        double HzOffset_end = 100;
        double depth = 80;
        double rod_extension = 150;
        // hole 
       


        // plate
        double plateThik = 4;
        string plateMaterial = "A36";
        double plateWidth = 160;
        string plate_perfix = "W";
        int plate_startNO = 101;
        string plate_name = "Plate";
        string plateProfile = null;
        Position.RotationEnum plateRotation;

        int removePlate1 = 0;
        int removePlate2 = 0;
        int removePlate3 = 0;
        int removePlate4= 0;

        //bolt
        private double bolt_sec_diameter = 20;
        private double tolerance_sec = 2;
        private string bolt_sec_screwdin = "4.6CSK";
        private double slotX_2 = 22;
        private double slotY_2 = 0;
        double edge_1 = 55;


        private void button1_Click(object sender, EventArgs e)
        {

            Picker inputs = new Picker();
            ModelObjectEnumerator rafter_1 = inputs.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, "Pick webs of first Rafter");
            ModelObjectEnumerator rafter_2 = inputs.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, "Pick webs of second Rafter");
            t3d.Point poinst_1 = inputs.PickPoint() as t3d.Point;
            t3d.Point poinst_2 = inputs.PickPoint() as t3d.Point;


        
            ArrayList rafter_1_web = new ArrayList();
            ArrayList rafter_2_web = new ArrayList();
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

            double web_1_width = 0;
            double web_2_width = 0;
            Beam web_1 = rafter_1_web[0] as Beam;
            Beam web_2 = rafter_2_web[0] as Beam;

            CoordinateSystem coordinate = web_1.GetCoordinateSystem();
            Vector directionX = new Vector(coordinate.AxisX);
            directionX.Normalize();
            Vector directionY = new Vector(coordinate.AxisY);
            directionY.Normalize();






         double dx_Boltedge_sec = 0;

        Position.RotationEnum plateRotation;

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


            double plateLength = 70;

            int NO_ofBolts_Y_sec = 1;
            double bolt_shift_2 = 0;
            int NO_ofBolts_X_sec = 1;
            dx_Boltedge_sec = plateWidth / 2;

            if (cb_sinleordouble.SelectedIndex == 0)
            {
                spacing = 0;
            }
            else
            {
                Beam rod3 = insertRod(rod_1_point1 - spacing * directionY, rod_1_point2 - spacing * directionY);
                Beam rod4 = insertRod(rod_2_point1 - spacing * directionY, rod_2_point2 - spacing * directionY);

                 NO_ofBolts_Y_sec = 2;


                Plate_1_Point1 = Plate_1_Point1 - spacing / 2 * directionY;

                Plate_2_Point1 = Plate_2_Point1 - spacing / 2 * directionY;

                Plate_3_Point1 = Plate_3_Point1 - spacing / 2 * directionY;

                Plate_4_Point1 = Plate_4_Point1 - spacing / 2 * directionY;


            }

            plateLength = edge_1 + edge_1 + spacing;
            plateProfile = "PL" + plateThik + "*" + plateLength;

            t3d.Point Plate_1_Point2 = Plate_1_Point1 + plateWidth * directionX;

            t3d.Point Plate_2_Point2 = Plate_2_Point1 + plateWidth * directionX;

            t3d.Point Plate_3_Point2 = Plate_3_Point1 + plateWidth * directionX;

            t3d.Point Plate_4_Point2 = Plate_4_Point1 + plateWidth * directionX;

            Beam plate1 = insertPlate(Plate_1_Point1, Plate_1_Point2);
            Beam plate2 = insertPlate(Plate_2_Point1, Plate_2_Point2);
            Beam plate3 = insertPlate(Plate_3_Point1, Plate_3_Point2);
            Beam plate4 = insertPlate(Plate_4_Point1, Plate_4_Point2);

            Beam holeWeb_1 = null, holeWeb_2 = null, holeWeb_3 = null, holeWeb_4 = null;
            LineSegment line_1 = new LineSegment(rod_1_point1, rod_1_point2);
            LineSegment line_2 = new LineSegment(rod_2_point1, rod_2_point2);

            for (int i = 0; i < rafter_1_web.Count; i++)
            {
                Beam current = rafter_1_web[i] as Beam;
                Solid solid = current.GetSolid();
                ArrayList p1 = solid.Intersect(line_1);
                ArrayList p2 = solid.Intersect(line_2);
                if (p1.Count >0)
                {
                    holeWeb_1 = current;
                }
                if (p2.Count>0)
                {
                    holeWeb_2 = current;
                }
            }
            for (int i = 0; i < rafter_2_web.Count; i++)
            {
                Beam current = rafter_2_web[i] as Beam;
                Solid solid = current.GetSolid();

                ArrayList p1 = solid.Intersect(line_1);
                ArrayList p2 = solid.Intersect(line_2);
                if (p1.Count>0 )
                {
                    holeWeb_3 = current;
                }
                 if (p2.Count>0)
                {
                    holeWeb_4 = current;
                }
            }
            List<double> Sx_sec = new List<double>();
            Sx_sec.Add(0);
            List<double> Sy_sec = new List<double>();
            Sy_sec.Add(spacing);



           

            BoltArray bolt1_plate = InsertBolt(Plate_1_Point1, Plate_1_Point2, plate1, plate1, dx_Boltedge_sec , bolt_sec_diameter,
                              tolerance_sec, bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", spacing.ToString()
                              , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, slotX_2,slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
              );


        BoltArray bolt2_plate = InsertBolt(Plate_2_Point1, Plate_2_Point2, plate2, plate2, dx_Boltedge_sec, bolt_sec_diameter,
                     tolerance_sec, bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", spacing.ToString()
                     , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, slotX_2, slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
     );

            BoltArray bolt3_plate = InsertBolt(Plate_3_Point1, Plate_3_Point2, plate3, plate3, dx_Boltedge_sec, bolt_sec_diameter,
                    tolerance_sec, bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", spacing.ToString()
                    , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, slotX_2, slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
    );

            BoltArray bolt4_plate = InsertBolt(Plate_4_Point1, Plate_4_Point2, plate4, plate4, dx_Boltedge_sec, bolt_sec_diameter,
                    tolerance_sec, bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", spacing.ToString()
                    , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, slotX_2, slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
    );


            BoltArray bolt1_web = InsertBolt(Plate_1_Point1, Plate_1_Point2, holeWeb_1, holeWeb_1, dx_Boltedge_sec, bolt_sec_diameter,
                             tolerance_sec, bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", spacing.ToString()
                             , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, slotX_2, slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
             );


            BoltArray bolt2_web = InsertBolt(Plate_2_Point1, Plate_2_Point2, holeWeb_3, holeWeb_3, dx_Boltedge_sec, bolt_sec_diameter,
                           tolerance_sec, bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", spacing.ToString()
                           , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, slotX_2, slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
           );

            BoltArray bolt3_web = InsertBolt(Plate_3_Point1, Plate_3_Point2, holeWeb_2, holeWeb_2, dx_Boltedge_sec, bolt_sec_diameter,
                           tolerance_sec, bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", spacing.ToString()
                           , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, slotX_2, slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
           );

            BoltArray bolt4_web = InsertBolt(Plate_4_Point1, Plate_4_Point2, holeWeb_4, holeWeb_4, dx_Boltedge_sec, bolt_sec_diameter,
                           tolerance_sec, bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", spacing.ToString()
                           , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, slotX_2, slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
           );
            insert_weld_allaround(holeWeb_3, plate2,4);
            insert_weld_allaround(holeWeb_1, plate1,4);
            insert_weld_allaround(holeWeb_2, plate3,4);
            insert_weld_allaround(holeWeb_4, plate4,4);
            if (removePlate1 == 1)
            {
                plate1.Delete();
            }
            if (removePlate2 == 1)
            {
                plate2.Delete();
            }
            if (removePlate3 == 1)
            {
                plate3.Delete();
            }
            if (removePlate4== 1)
            {
                plate4.Delete();
            }
            //plate1.Delete();
            //plate2.Delete();
            //plate3.Delete();
            //plate4.Delete();
            myModel.CommitChanges();
        }
        public Weld insert_weld_allaround(Part Main_part, Part Secandary_part, double below)
        {
            Weld weld = new Weld();
            weld.MainObject = Main_part;
            weld.SecondaryObject = Secandary_part;
            weld.TypeAbove = BaseWeld.WeldTypeEnum.WELD_TYPE_NONE;
            weld.SizeAbove = 0;
            weld.SizeBelow = below;
            weld.TypeBelow = BaseWeld.WeldTypeEnum.WELD_TYPE_FILLET;

            weld.ShopWeld = true;
            weld.AroundWeld = true;
            weld.Insert();
            return weld;
        }
        public Beam insertRod(Point start_point, Point end_point)
        {
            Beam beam = new Beam();
            beam.StartPoint = start_point;
            beam.EndPoint = end_point;
            beam.Profile.ProfileString =rod_profile ;
            beam.Material.MaterialString = rod_material;
            beam.StartPointOffset.Dx =-rod_extension;
            beam.EndPointOffset.Dx = rod_extension;
            beam.PartNumber.Prefix="W";
            beam.PartNumber.StartNumber = 101;
            beam.AssemblyNumber.StartNumber = rod_startNO;
            beam.AssemblyNumber.Prefix = rod_perfix;
            beam.Position.Depth = Position.DepthEnum.MIDDLE;
            beam.Position.Plane = Position.PlaneEnum.MIDDLE;
            beam.Position.Rotation = Position.RotationEnum.TOP;
            beam.Name = rod_name;
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

            beam.PartNumber.Prefix = plate_perfix;
            beam.PartNumber.StartNumber = plate_startNO;
            beam.AssemblyNumber.StartNumber = 4001;
            beam.AssemblyNumber.Prefix = "";
            beam.Position.Depth = Position.DepthEnum.MIDDLE;
            beam.Position.Plane = Position.PlaneEnum.MIDDLE;
            beam.Position.Rotation = plateRotation;
            beam.Name = plate_name;
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

        private BoltArray InsertBolt(Point platePoint1, Point plateMidPoint, Part mainPart, Part plate, double dx, double boltSize,
            double tolerance, string boltStandard, int no_bolt_x, int no_bolt_y, string spacing_x, string spacing_y,
              BoltArray.BoltTypeEnum boltType, bool sloyInMainPart, bool sloyInSecnPart, double slot_X, double slot_y, bool Washer2, bool nut2
           , double shift, List<double> Sx, List<double> Sy)
        {



            BoltArray boltArray = new BoltArray();
            boltArray.FirstPosition = plateMidPoint;
            boltArray.SecondPosition = platePoint1;
            boltArray.PartToBeBolted = plate;
            boltArray.PartToBoltTo = mainPart;

            string[] spacing_X_array = spacing_x.Split(' ');
            string[] spacing_Y_array = spacing_y.Split(' ');


            if (no_bolt_x > 1)
            {
                for (int i = 0; i < no_bolt_x - 1; i++)
                {
                    boltArray.AddBoltDistX(Sx[i]);
                }
            }
            else
            {
                boltArray.AddBoltDistX(0);

            }

            if (no_bolt_y > 1)
            {
                for (int i = 0; i < no_bolt_y - 1; i++)
                {
                    boltArray.AddBoltDistY(Sy[i]);
                }
            }
            else
            {
                boltArray.AddBoltDistY(0);

            }
            //for (int i = 0; i < no_bolt_x - 1; i++)
            //{
            //    boltArray.AddBoltDistX(double.Parse(spacing_X_array[i]));

            //}
            //for (int i = 0; i < no_bolt_y - 1; i++)
            //{
            //    boltArray.AddBoltDistY(double.Parse(spacing_Y_array[i]));

            //}
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
            boltArray.Washer2 = Washer2;
            boltArray.Nut2 = nut2;
            boltArray.Bolt = false;

            boltArray.HoleType = BoltGroup.BoltHoleTypeEnum.HOLE_TYPE_SLOTTED;

            boltArray.StartPointOffset.Dz = shift;
            boltArray.EndPointOffset.Dz = shift;
            boltArray.Hole3 = false;
            boltArray.Hole4 = false;
            boltArray.Hole5 = false;
            boltArray.HoleType = BoltGroup.BoltHoleTypeEnum.HOLE_TYPE_SLOTTED;
            boltArray.Washer1 = false;
            boltArray.Nut1 = false;
            boltArray.Insert();
            return boltArray;
        }
    }
}
