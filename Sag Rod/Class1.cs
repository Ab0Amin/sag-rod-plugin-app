using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using t3d = Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Plugins;
using Tekla.Structures.Solid;
using System.Linq;
using System.Windows.Forms;
public class StructuresData3
{

    //[Tekla.Structures.Plugins.StructuresField("P1")]
    //public double Parameter1;

    // rod
    [StructuresField("rod_profile")]
    public string rod_profile;
    [StructuresField("rod_material")]
    public string rod_material;
    [StructuresField("rod_perfix")]
    public string rod_perfix;
    [StructuresField("rod_name")]
    public string rod_name;
    [StructuresField("rod_startNO")]
    public int rod_startNO;

    // dim
    [StructuresField("spacing")]
    public double spacing;
    [StructuresField("HzOffset_start")]
    public double HzOffset_start;
    [StructuresField("HzOffset_end")]
    public double HzOffset_end;
    [StructuresField("depth")]
    public double depth;
    [StructuresField("rod_extension")]
    public double rod_extension;

    //plate
    [StructuresField("plateThik")]
    public double plateThik;
    [StructuresField("plateMaterial")]
    public string plateMaterial;
    [StructuresField("plateWidth")]
    public double plateWidth;
    [StructuresField("plate_perfix")]
    public string plate_perfix;
    [StructuresField("plate_startNO")]
    public int plate_startNO;
    [StructuresField("plate_name")]
    public string plate_name;
    [StructuresField("plateProfile")]
    public string plateProfile;
    [StructuresField("plateRotation")]
    public Position.RotationEnum plateRotation;

    // remove plates
    [StructuresField("removePlate1")]
    public int removePlate1;
    [StructuresField("removePlate2")]
    public int removePlate2;
    [StructuresField("removePlate3")]
    public int removePlate3;
    [StructuresField("removePlate4")]
    public int removePlate4;

    // bolt
    [StructuresField("bolt_sec_screwdin")]
    public string bolt_sec_screwdin;
    [StructuresField("bolt_sec_diameter")]
    public double bolt_sec_diameter;
    [StructuresField("tolerance_sec")]
    public double tolerance_sec;
    [StructuresField("slotX_2")]
    public double slotX_2;
    [StructuresField("slotY_2")]
    public double slotY_2;
    [StructuresField("edge_1")]
    public double edge_1;
    [StructuresField("cb_sinleordouble")]
    public int cb_sinleordouble;

   

}

[Plugin("Rod_Bracing")]
[PluginUserInterface(Rod_Bracing.UserInterfaceDefinition.Plugin3)]






public class Rod_Bracing : PluginBase
{

    private readonly StructuresData3 data;
    private readonly Tekla.Structures.Model.Model myModel;

    public Rod_Bracing(StructuresData3 data)
    {
        this.data = data;
        this.myModel = new Tekla.Structures.Model.Model();
    }

    public override List<PluginBase.InputDefinition> DefineInput()
    {
        List<PluginBase.InputDefinition> inputDefinitionList = new List<PluginBase.InputDefinition>();
        //Picker picker = new Picker();
        //Part mainPart = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick Main Part") as Part;
        //Part secendaryPart = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Pick Secandry Part") as Part;

        Picker inputs = new Picker();
        ModelObjectEnumerator rafter_1 = inputs.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, "Pick webs of first Rafter");
        ModelObjectEnumerator rafter_2 = inputs.PickObjects(Picker.PickObjectsEnum.PICK_N_PARTS, "Pick webs of second Rafter");
        t3d.Point poinst_1 = inputs.PickPoint() as t3d.Point;
        t3d.Point poinst_2 = inputs.PickPoint() as t3d.Point;
        ArrayList r1 = new ArrayList();
        ArrayList r2 = new ArrayList();
        while (rafter_1.MoveNext())
        {
            Beam b = rafter_1.Current as Beam;
            Identifier identifier = b.Identifier;

            if (identifier != null)
            {

                r1.Add(identifier);
            }
        }
        while (rafter_2.MoveNext())
        {
            Beam b = rafter_2.Current as Beam;
            Identifier identifier = b.Identifier;

            if (identifier != null)
            {

                r2.Add(identifier);
            }
        }
        PluginBase.InputDefinition inputDefinition1 = new PluginBase.InputDefinition(r1 );
        PluginBase.InputDefinition inputDefinition2 = new PluginBase.InputDefinition(r2);
        PluginBase.InputDefinition inputDefinition3 = new PluginBase.InputDefinition(poinst_1);
        PluginBase.InputDefinition inputDefinition4= new PluginBase.InputDefinition(poinst_2);
        inputDefinitionList.Add(inputDefinition1);
        inputDefinitionList.Add(inputDefinition2);
        inputDefinitionList.Add(inputDefinition3);
        inputDefinitionList.Add(inputDefinition4);
        return inputDefinitionList;
    }

    public override bool Run(List<PluginBase.InputDefinition> Input)
    {
        try
        {
            #region defults
            //  rod
            if (this.IsDefaultValue(this.data.rod_profile))
                this.data.rod_profile = "ROD16";
            if (this.IsDefaultValue(this.data.rod_material))
                this.data.rod_material = "A36";
            if (this.IsDefaultValue(this.data.rod_perfix))
                this.data.rod_perfix = "";
            if (this.IsDefaultValue(this.data.rod_startNO))
                this.data.rod_startNO = 4001;
            if (this.IsDefaultValue(this.data.rod_name))
                this.data.rod_name = "HORBRACE";

            // dim
            if (this.IsDefaultValue(this.data.spacing))
                this.data.spacing = 70;

            if (this.IsDefaultValue(this.data.HzOffset_start))
                this.data.HzOffset_start = 150;

            if (this.IsDefaultValue(this.data.HzOffset_end))
                this.data.HzOffset_end = 150;

            if (this.IsDefaultValue(this.data.depth))
                this.data.depth = 50;

            if (this.IsDefaultValue(this.data.rod_extension))
                this.data.rod_extension = 150;

            //plate

            if (this.IsDefaultValue(this.data.plateThik))
                this.data.plateThik = 4;
            if (this.IsDefaultValue(this.data.plateMaterial))
                this.data.plateMaterial = "A36";
            if (this.IsDefaultValue(this.data.plateWidth))
                this.data.plateWidth = 140;
            if (this.IsDefaultValue(this.data.plate_perfix))
                this.data.plate_perfix = "W";
            if (this.IsDefaultValue(this.data.plate_startNO))
                this.data.plate_startNO = 101;

            if (this.IsDefaultValue(this.data.plate_name))
                this.data.plate_name = "PLATE";
            if (this.IsDefaultValue(this.data.plateProfile))
                this.data.plateProfile = "PL4*140";
            //if (this.IsDefaultValue(this.data.plateRotation))
            //    this.data.plateRotation =Position.RotationEnum.BACK;

            // remove 
            if (this.IsDefaultValue(this.data.removePlate1))
                this.data.removePlate1 = 0;

            if (this.IsDefaultValue(this.data.removePlate2))
                this.data.removePlate2 = 0;
            if (this.IsDefaultValue(this.data.removePlate3))
                this.data.removePlate3 = 0;
            if (this.IsDefaultValue(this.data.removePlate4))
                this.data.removePlate4 = 0;

            // bolt
            if (this.IsDefaultValue(this.data.bolt_sec_diameter))
                this.data.bolt_sec_diameter = 20;
            if (this.IsDefaultValue(this.data.tolerance_sec))
                this.data.tolerance_sec = 2;
            if (this.IsDefaultValue(this.data.bolt_sec_screwdin))
                this.data.bolt_sec_screwdin = "4.6CSK";
            if (this.IsDefaultValue(this.data.slotX_2))
                this.data.slotX_2 = 22;
            if (this.IsDefaultValue(this.data.slotY_2))
                this.data.slotY_2 = 0;
            if (this.IsDefaultValue(this.data.edge_1))
                this.data.edge_1 = 35;
            if (this.IsDefaultValue(this.data.cb_sinleordouble))
                this.data.cb_sinleordouble = 0;

            
            #endregion

            //this.createconnection((Part)this.myModel.SelectModelObject((Identifier)Input[0].GetInput()), (Part)this.myModel.SelectModelObject((Identifier)Input[1].GetInput()));
            this.createconnection((ArrayList)Input[0].GetInput() , (ArrayList)Input[1].GetInput() , (t3d.Point)Input[2].GetInput() , (t3d.Point)Input[3].GetInput());
        }
        catch (Exception e)
        {

            MessageBox.Show(e.ToString());
        }
        return true;
    }

    public void createconnection(ArrayList rafter_1_web_id, ArrayList rafter_2_web_id, t3d.Point poinst_1, t3d.Point poinst_2)
    {
        ArrayList rafter_1_web = new ArrayList();
        ArrayList rafter_2_web = new ArrayList();
        foreach (Identifier ID in rafter_1_web_id)
        {
            Part part1 = (Part)this.myModel.SelectModelObject(ID);
            if (part1 != null)
            {
                rafter_1_web.Add(part1);
            }
        }
        foreach (Identifier ID in rafter_2_web_id)
        {
            Part part1 = (Part)this.myModel.SelectModelObject(ID);
            if (part1 != null)
            {
                rafter_2_web.Add(part1);
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


        Point orgin_1 = web_1.GetCenterLine(false)[0] as t3d.Point;
        web_1.GetReportProperty("HEIGHT", ref web_1_width);
        orgin_1 = orgin_1 - (web_1_width / 2 - data.depth) * -1 * directionY;

        data.plateRotation = web_1.Position.Rotation;

        Point orgin_2 = web_2.GetCenterLine(false)[0] as t3d.Point;
        web_2.GetReportProperty("HEIGHT", ref web_2_width);
        orgin_2 = orgin_2 - (web_2_width / 2 - data.depth) * -1 * directionY;


        GeometricPlane HZplane1 = new GeometricPlane(orgin_1, directionX, directionX.Cross(directionY));
        GeometricPlane vlPlane1 = new GeometricPlane(orgin_1, directionX, (directionY));
        GeometricPlane vlPlane2 = new GeometricPlane(orgin_2, directionX, (directionY));


        t3d.Point projected_point_1 = Projection.PointToPlane(poinst_1, HZplane1);
        t3d.Point projected_point_2 = Projection.PointToPlane(poinst_2, HZplane1);

        projected_point_1 = projected_point_1 + directionX * data.HzOffset_start;
        projected_point_2 = projected_point_2 - directionX * data.HzOffset_end;

        t3d.Point rod_1_point1 = Projection.PointToPlane(projected_point_1, vlPlane1);
        t3d.Point rod_2_point1 = Projection.PointToPlane(projected_point_2, vlPlane1);

        t3d.Point rod_2_point2 = Projection.PointToPlane(projected_point_1, vlPlane2);
        t3d.Point rod_1_point2 = Projection.PointToPlane(projected_point_2, vlPlane2);

        Beam rod1 = insertRod(rod_1_point1, rod_1_point2);
        Beam rod2 = insertRod(rod_2_point1, rod_2_point2);

        t3d.Point Plate_1_Point1 = rod_1_point1 - data.plateWidth / 2 * directionX;

        t3d.Point Plate_2_Point1 = rod_1_point2 - data.plateWidth / 2 * directionX;

        t3d.Point Plate_3_Point1 = rod_2_point1 - data.plateWidth / 2 * directionX;

        t3d.Point Plate_4_Point1 = rod_2_point2 - data.plateWidth / 2 * directionX;


        double plateLength = 70;

        int NO_ofBolts_Y_sec = 1;
        double bolt_shift_2 = 0;
        int NO_ofBolts_X_sec = 1;
        dx_Boltedge_sec = data.plateWidth / 2;

        if (data.cb_sinleordouble == 0)
        {
            data.spacing = 0;
        }
        else
        {
            Beam rod3 = insertRod(rod_1_point1 - data.spacing * directionY, rod_1_point2 - data.spacing * directionY);
            Beam rod4 = insertRod(rod_2_point1 - data.spacing * directionY, rod_2_point2 - data.spacing * directionY);

            NO_ofBolts_Y_sec = 2;


            Plate_1_Point1 = Plate_1_Point1 - data.spacing / 2 * directionY;

            Plate_2_Point1 = Plate_2_Point1 - data.spacing / 2 * directionY;

            Plate_3_Point1 = Plate_3_Point1 - data.spacing / 2 * directionY;

            Plate_4_Point1 = Plate_4_Point1 - data.spacing / 2 * directionY;


        }

        plateLength = data.edge_1 + data.edge_1 + data.spacing;
        data.plateProfile = "PL" + data.plateThik + "*" + plateLength;

        t3d.Point Plate_1_Point2 = Plate_1_Point1 + data.plateWidth * directionX;

        t3d.Point Plate_2_Point2 = Plate_2_Point1 + data.plateWidth * directionX;

        t3d.Point Plate_3_Point2 = Plate_3_Point1 + data.plateWidth * directionX;

        t3d.Point Plate_4_Point2 = Plate_4_Point1 + data.plateWidth * directionX;

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
            if (p1.Count > 0)
            {
                holeWeb_1 = current;
            }
            if (p2.Count > 0)
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
            if (p1.Count > 0)
            {
                holeWeb_3 = current;
            }
            if (p2.Count > 0)
            {
                holeWeb_4 = current;
            }
        }
        List<double> Sx_sec = new List<double>();
        Sx_sec.Add(0);
        List<double> Sy_sec = new List<double>();
        Sy_sec.Add(data.spacing);





        BoltArray bolt1_plate = InsertBolt(Plate_1_Point1, Plate_1_Point2, plate1, plate1, dx_Boltedge_sec, data.bolt_sec_diameter,
                          data.tolerance_sec, data.bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", data.spacing.ToString()
                          , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, data.slotX_2, data.slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
          );


        BoltArray bolt2_plate = InsertBolt(Plate_2_Point1, Plate_2_Point2, plate2, plate2, dx_Boltedge_sec, data.bolt_sec_diameter,
                    data.tolerance_sec, data.bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", data.spacing.ToString()
                     , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, data.slotX_2, data.slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
     );

        BoltArray bolt3_plate = InsertBolt(Plate_3_Point1, Plate_3_Point2, plate3, plate3, dx_Boltedge_sec, data.bolt_sec_diameter,
                data.tolerance_sec, data.bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", data.spacing.ToString()
                , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, data.slotX_2, data.slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
);

        BoltArray bolt4_plate = InsertBolt(Plate_4_Point1, Plate_4_Point2, plate4, plate4, dx_Boltedge_sec, data.bolt_sec_diameter,
                data.tolerance_sec, data.bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", data.spacing.ToString()
                , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, data.slotX_2, data.slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
);


        BoltArray bolt1_web = InsertBolt(Plate_1_Point1, Plate_1_Point2, holeWeb_1, holeWeb_1, dx_Boltedge_sec, data.bolt_sec_diameter,
                        data.tolerance_sec, data.bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", data.spacing.ToString()
                         , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, data.slotX_2, data.slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
         );


        BoltArray bolt2_web = InsertBolt(Plate_2_Point1, Plate_2_Point2, holeWeb_3, holeWeb_3, dx_Boltedge_sec, data.bolt_sec_diameter,
                      data.tolerance_sec, data.bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", data.spacing.ToString()
                       , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, data.slotX_2, data.slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
       );

        BoltArray bolt3_web = InsertBolt(Plate_3_Point1, Plate_3_Point2, holeWeb_2, holeWeb_2, dx_Boltedge_sec, data.bolt_sec_diameter,
                      data.tolerance_sec, data.bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", data.spacing.ToString()
                       , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, data.slotX_2, data.slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
       );

        BoltArray bolt4_web = InsertBolt(Plate_4_Point1, Plate_4_Point2, holeWeb_4, holeWeb_4, dx_Boltedge_sec, data.bolt_sec_diameter,
                       data.tolerance_sec, data.bolt_sec_screwdin, NO_ofBolts_X_sec, NO_ofBolts_Y_sec, "", data.spacing.ToString()
                       , BoltGroup.BoltTypeEnum.BOLT_TYPE_SITE, true, true, data.slotX_2, data.slotY_2, false, false, bolt_shift_2, Sx_sec, Sy_sec
       );
        insert_weld_allaround(holeWeb_3, plate2, 4);
        insert_weld_allaround(holeWeb_1, plate1, 4);
        insert_weld_allaround(holeWeb_2, plate3, 4);
        insert_weld_allaround(holeWeb_4, plate4, 4);
        if (data.removePlate1 == 1)
        {
            plate1.Delete();
        }
        if (data.removePlate2 == 1)
        {
            plate2.Delete();
        }
        if (data.removePlate3 == 1)
        {
            plate3.Delete();
        }
        if (data.removePlate4 == 1)
        {
            plate4.Delete();
        }
        //plate1.Delete();
        //plate2.Delete();
        //plate3.Delete();
        //plate4.Delete();
      //  myModel.CommitChanges();
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
        beam.Profile.ProfileString = data.rod_profile;
        beam.Material.MaterialString = data.rod_material;
        beam.StartPointOffset.Dx = -data.rod_extension;
        beam.EndPointOffset.Dx = data.rod_extension;
        beam.PartNumber.Prefix = "L";
        beam.PartNumber.StartNumber = 101;
        beam.AssemblyNumber.StartNumber = data.rod_startNO;
        beam.AssemblyNumber.Prefix = data.rod_perfix;
        beam.Name = data.rod_name;
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
        beam.Profile.ProfileString = data.plateProfile;
        beam.Material.MaterialString = data.plateMaterial;

        beam.PartNumber.Prefix = data.plate_perfix;
        beam.PartNumber.StartNumber = data.plate_startNO;
        beam.AssemblyNumber.StartNumber = 4001;
        beam.AssemblyNumber.Prefix = "";
        beam.Name = data.plate_name;
        beam.Position.Depth = Position.DepthEnum.MIDDLE;
        beam.Position.Plane = Position.PlaneEnum.MIDDLE;
        beam.Position.Rotation = data.plateRotation;
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
        boltArray.Position.Rotation = data.plateRotation;

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


    public class UserInterfaceDefinition
    {

        public const string Plugin1 = @"" +
         @"page(""TeklaStructures"","""")" + "\n" +
          "{\n" +
          "    plugin(1, Rod_Bracing)\n" +
          "    {\n" +
         @"        tab_page(""Beam test"", ""Parametri_1"", 1)" + "\n" +
          "        {\n" +
         @"            parameter(""Length factor"", ""platethick"", distance, number, 1)" + "\n" +
          "        }\n" +
          "    }\n" +
          "}\n";

        public const string Plugin3 =
            "page(\"TeklaStructures\",\"\")\n   " +
            " {\n    joint(1, Rod_Bracing)\n   " +

            " {\n      tab_page(\"1\", \" Picture \", 1)\n   " +


            "     {\n      " +


             //plate lables
             "   attribute(\"\", \"t\", label, \"%s\", none, none, \"0\", \"0\", 140, 9)\n     " +
            "       attribute(\"\", \"b\", label, \"%s\", none, none, \"0\", \"0\", 190, 9)\n         " +
            "   attribute(\"\", \"Prefix\", label, \"%s\", none, none, \"0\", \"0\", 285, 9)\n         " +
            "   attribute(\"\", \"Start_NO\", label, \"%s\", none, none, \"0\", \"0\", 365, 9)\n       " +
            "     attribute(\"\", \"Matrial\", label, \"%s\", none, none, \"0\", \"0\", 470, 9)\n     " +
            "       attribute(\"\", \"name\", label, \"%s\", none, none, \"0\", \"0\", 610, 9)\n      " +
            
            "      attribute(\"\", \"Rod Profile\", label, \"%s\", none, none, \"0\", \"0\", 15, 34)\n       " +
            "      attribute(\"\", \"Plate\", label, \"%s\", none, none, \"0\", \"0\", 15, 75)\n       " +

            //rod prameterss
            "     parameter(\"\", \"rod_profile\", profile, number, 125, 34, 115)\n         " +
            "     parameter(\"\", \"rod_perfix\", string, text, 295, 34, 40)\n         " +
            "   parameter(\"\", \"rod_startNO\", integer, number, 385, 34, 70)\n   " +
            "         parameter(\"\", \"rod_material\", material, text, 490, 34, 100)\n   " +
            "         parameter(\"\", \"rod_name\", string, text, 650, 34, 100)\n  " +

                  //plate prameterss
                  "     parameter(\"\", \"plateThik\", distance, number, 125, 75, 40)\n         " +
                  "     parameter(\"\", \"plateWidth\", distance, number, 205, 75, 50)\n         " +

                  "     parameter(\"\", \"plate_perfix\", string, text, 295, 75, 40)\n         " +
                  "   parameter(\"\", \"plate_startNO\", integer, number, 385, 75, 70)\n   " +
                  "         parameter(\"\", \"plateMaterial\", material, text, 490, 75, 100)\n   " +
                  "         parameter(\"\", \"plate_name\", string, text, 650, 75, 100)\n  " +


          // pics

          "   picture(\"sagRodPlane\", 0, 0, 104, 132)\n      " +
          "   picture(\"sagrodElevasion\", 0, 0, 452, 132)\n      " +
          "   picture(\"sagRodBltEdge\", 0, 0, 372, 371)\n      " +

              // dim

              "         parameter(\"\", \"HzOffset_end\", distance, number, 42, 130, 50)\n  " +
              "         parameter(\"\", \"HzOffset_start\", distance, number, 42, 270, 50)\n  " +
              "         parameter(\"\", \"rod_extension\", distance, number, 300, 150, 50)\n  " +
              "         parameter(\"\", \"depth\", distance, number, 385, 170, 50)\n  " +
              "         parameter(\"\", \"spacing\", distance, number, 385, 220, 50)\n  " +

              "   attribute(\"cb_sinleordouble\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 390, 270, 100)\n  " +
                  "  {\n     " +

                  "   value(\"Single\", 1)\n      " +
                  "  value(\"Double\", 0)\n  " +
                  "  }\n         " +


             //remove plates 


             "   attribute(\"removePlate2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 25, 160, 70)\n  " +
                  "  {\n     " +

                  "   value(\"Plate\", 1)\n      " +
                  "  value(\"None\", 0)\n  " +
                  "  }\n         " +

            "   attribute(\"removePlate4\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 25, 300, 70)\n  " +
                  "  {\n     " +

                  "   value(\"Plate\", 1)\n      " +
                  "  value(\"None\", 0)\n  " +
                  "  }\n         " +

            "   attribute(\"removePlate3\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 300, 118, 70)\n  " +
                  "  {\n     " +

                  "   value(\"Plate\", 1)\n      " +
                  "  value(\"None\", 0)\n  " +
                  "  }\n         " +

            "   attribute(\"removePlate1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 290, 290, 70)\n  " +
                  "  {\n     " +

                  "   value(\"Plate\", 1)\n      " +
                  "  value(\"None\", 0)\n  " +
                  "  }\n         " +

             //bolt
             "   attribute(\"\", \"Bolt Standard\", label, \"%s\", none, none, \"0\", \"0\", 40, 380)\n     " +
             "   attribute(\"\", \"Bolt Size\", label, \"%s\", none, none, \"0\", \"0\", 40, 410)\n     " +
             "   attribute(\"\", \"Tolerance\", label, \"%s\", none, none, \"0\", \"0\", 40, 440)\n     " +
             "   attribute(\"\", \"X\", label, \"%s\", none, none, \"0\", \"0\", 120, 470)\n     " +
             "   attribute(\"\", \"Y\", label, \"%s\", none, none, \"0\", \"0\", 210, 470)\n     " +
             "   attribute(\"\", \"Slot\", label, \"%s\", none, none, \"0\", \"0\", 40, 495)\n     " +


            "         parameter(\"\", \"bolt_sec_screwdin\", bolt_standard, text, 170, 380, 100)\n  " +
            "         parameter(\"\", \"bolt_sec_diameter\", bolt_size, number, 170,410, 100)\n  " +
            "         parameter(\"\", \"tolerance_sec\", distance, number, 170,440, 100)\n  " +
            "         parameter(\"\", \"slotX_2\", distance, number, 102,495, 50)\n  " +
            "         parameter(\"\", \"slotY_2\", distance, number, 190,495, 50)\n  " +
            "         parameter(\"\", \"edge_1\", distance, number, 310,390, 50)\n  " +



            //            // plate chanfer
            //           "   attribute(\"cb_polybeamChanfer\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 80, 210, 70)\n  " +
            //      "  {\n     " +

            //      "   value(\"arc\", 1)\n      " +
            //      "  value(\"line\", 0)\n  " +
            //      "  value(\"none\", 0)\n  " +
            //      "  }\n         " +
            //      "         parameter(\"\", \"polyBeam_chanfer_x\", distance, number, 80, 250, 70)\n  " +
            //      "         parameter(\"\", \"polyBeam_chanfer_y\", distance, number, 80, 280, 70)\n  " +

            //           // stiff chanfer
            //           "   attribute(\"cb_stiffChanfer\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 370, 155, 70)\n  " +
            //      "  {\n     " +

            //      "   value(\"arc\", 0)\n      " +
            //      "  value(\"line\", 1)\n  " +
            //      "  value(\"none\", 0)\n  " +
            //      "  }\n         " +
            //      "         parameter(\"\", \"stiff_chanfer_x\", distance, number, 370, 195, 70)\n  " +
            //      "         parameter(\"\", \"stiff_chanfer_y\", distance, number, 370, 230, 70)\n  " +
            //      //plate stiff pic
            //      "  picture(\"plate_stiff_anglePlugin_peb\", 0, 0, 185, 155)\n      " +

            //      //connection shift
            //      "  picture(\"xs_detail_64_point_def\", 0, 0, 725, 155)\n      " +
            //      "  attribute(\"\", \"Connection shift\", label, \"%s\", none, none, \"0\", \"0\", 545, 260)\n     " +
            //      "  parameter(\"\", \"Connection_shift\", distance, number, 695, 260, 70)\n  " +



            // // primary bolt

            // //labels
            // "   attribute(\"\", \"Primary Bolts\", label, \"%s\", none, none, \"0\", \"0\", 105, 350)\n     " +
            // "   attribute(\"\", \"Bolt Standard\", label, \"%s\", none, none, \"0\", \"0\", 40, 380)\n     " +
            // "   attribute(\"\", \"Bolt Size\", label, \"%s\", none, none, \"0\", \"0\", 40, 410)\n     " +
            // "   attribute(\"\", \"Tolerance\", label, \"%s\", none, none, \"0\", \"0\", 40, 440)\n     " +
            // "   attribute(\"\", \"Workshop/Site\", label, \"%s\", none, none, \"0\", \"0\", 40, 470)\n     " +
            // "   attribute(\"\", \"Washer\", label, \"%s\", none, none, \"0\", \"0\", 40, 510)\n     " +
            // "   attribute(\"\", \"Nut\", label, \"%s\", none, none, \"0\", \"0\", 200, 510)\n     " +
            // "   attribute(\"\", \"X\", label, \"%s\", none, none, \"0\", \"0\", 95, 585)\n     " +
            // "   attribute(\"\", \"Y\", label, \"%s\", none, none, \"0\", \"0\", 185, 585)\n     " +
            // "   attribute(\"\", \"Slot\", label, \"%s\", none, none, \"0\", \"0\", 15, 615)\n     " +
            // "   attribute(\"\", \"Weld Size\", label, \"%s\", none, none, \"0\", \"0\", 270, 650)\n     " +
            // "   attribute(\"\", \"Bolt Shift\", label, \"%s\", none, none, \"0\", \"0\", 175, 720)\n     " +

            //// parameters
            //"         parameter(\"\", \"bolt_main_screwdin\", bolt_standard, text, 170, 380, 100)\n  " +
            //"         parameter(\"\", \"bolt_main_diameter\", bolt_size, number, 170,410, 100)\n  " +
            //"         parameter(\"\", \"tolerance_main\", distance, number, 170,440, 100)\n  " +
            //"   attribute(\"cb_workshop_1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 170, 470, 100)\n  " +
            //"  {\n     " +
            //"   value(\"Workshop\", 0)\n      " +
            //"  value(\"Site\", 1)\n  " +
            //"  }\n            " +
            //   "   attribute(\"cm_washerNo_1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 30, 550, 100)\n  " +
            //"  {\n     " +
            //"   value(\"1 Washer\", 1)\n      " +
            //"  value(\"2 Washer\", 0)\n  " +
            //"  }\n            " +

            //    "   attribute(\"cm_nutNo_1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 175, 550, 100)\n  " +
            //"  {\n     " +
            //"   value(\"1 Nut\", 1)\n      " +
            //"  value(\"2 Nut\", 0)\n  " +
            //"  }\n" +

            //"         parameter(\"\", \"slotX_1\", distance, number, 75,610, 50)\n  " +
            //"         parameter(\"\", \"slotY_1\", distance, number, 165,610, 50)\n  " +


            //    "   attribute(\"cb_sloted_1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 250, 610, 80)\n  " +
            //"  {\n     " +
            //"   value(\"Plate\", 0)\n      " +
            //"  value(\"Beam\", 0)\n  " +
            //"   value(\"none\", 1)\n      " +

            //"  }\n" +

            //    "   attribute(\"cb_weldedBolt_1\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 20, 675, 150)\n  " +
            //"  {\n     " +
            // "   value(\"Beam_to_Beam_Clip1.xbm\", 1)\n      " +
            //"  value(\"Beam_to_Beam_Clip2.xbm\", 0)\n  " +
            //"  }\n" +

            //"         parameter(\"\", \"polybeam_weldWithMain\", distance, number, 265,675, 50)\n  " +
            //"         parameter(\"\", \"bolt_shift_1\", distance, number, 265,720, 50)\n  " +
            ////pic
            //"  picture(\"bolt_anglepeb_plugin\", 0, 0, 328, 374)\n      " +


            // // sec bolt

            // //labels
            // "   attribute(\"\", \"Secondary Bolts\", label, \"%s\", none, none, \"0\", \"0\", 670, 350)\n     " +
            // "   attribute(\"\", \"Bolt Standard\", label, \"%s\", none, none, \"0\", \"0\", 625, 380)\n     " +
            // "   attribute(\"\", \"Bolt Size\", label, \"%s\", none, none, \"0\", \"0\", 625, 410)\n     " +
            // "   attribute(\"\", \"Tolerance\", label, \"%s\", none, none, \"0\", \"0\", 625, 440)\n     " +
            // "   attribute(\"\", \"Workshop/Site\", label, \"%s\", none, none, \"0\", \"0\", 625, 470)\n     " +
            // "   attribute(\"\", \"Washer\", label, \"%s\", none, none, \"0\", \"0\", 625, 510)\n     " +
            // "   attribute(\"\", \"Nut\", label, \"%s\", none, none, \"0\", \"0\", 785, 510)\n     " +
            // "   attribute(\"\", \"X\", label, \"%s\", none, none, \"0\", \"0\", 680, 585)\n     " +
            // "   attribute(\"\", \"Y\", label, \"%s\", none, none, \"0\", \"0\", 770, 585)\n     " +
            // "   attribute(\"\", \"Slot\", label, \"%s\", none, none, \"0\", \"0\", 600, 615)\n     " +
            // "   attribute(\"\", \"Weld Size\", label, \"%s\", none, none, \"0\", \"0\", 855, 650)\n     " +
            // "   attribute(\"\", \"Bolt Shift\", label, \"%s\", none, none, \"0\", \"0\", 760, 720)\n     " +

            //// parameters
            //"         parameter(\"\", \"bolt_sec_screwdin\", bolt_standard, text, 755, 380, 100)\n  " +
            //"         parameter(\"\", \"bolt_sec_diameter\", bolt_size, number, 755,410, 100)\n  " +
            //"         parameter(\"\", \"tolerance_sec\", distance, number, 755,440, 100)\n  " +
            //"   attribute(\"cb_workshop_2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 755, 470, 100)\n  " +
            //"  {\n     " +
            //"   value(\"Workshop\", 1)\n      " +
            //"  value(\"Site\", 0)\n  " +
            //"  }\n            " +
            //   "   attribute(\"cm_washerNo_2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 625, 550, 100)\n  " +
            //"  {\n     " +
            //"   value(\"1 Washer\", 1)\n      " +
            //"  value(\"2 Washer\", 0)\n  " +
            //"  }\n            " +

            //    "   attribute(\"cm_nutNo_2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 760, 550, 100)\n  " +
            //"  {\n     " +
            //"   value(\"1 Nut\", 1)\n      " +
            //"  value(\"2 Nut\", 0)\n  " +
            //"  }\n" +

            //"         parameter(\"\", \"slotX_2\", distance, number, 690,610, 50)\n  " +
            //"         parameter(\"\", \"slotY_2\", distance, number, 770,610, 50)\n  " +


            //    "   attribute(\"cb_solted_2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 860, 610, 80)\n  " +
            //"  {\n     " +
            //"   value(\"Plate\", 0)\n      " +
            //"  value(\"Beam\", 0)\n  " +
            //"  value(\"none\", 1)\n  " +
            //"  }\n" +

            //    "   attribute(\"cb_weldedBolt_2\", \"\", option, \"%s\", none, none, \"0.0\", \"0.0\", 565, 675, 150)\n  " +
            //"  {\n     " +
            //"   value(\"Beam_to_Beam_Clip1.xbm\", 1)\n      " +
            //"  value(\"Beam_to_Beam_Clip3.xbm,\", 0)\n  " +
            //"  }\n" +

            //"         parameter(\"\", \"polybeam_weldWithSec\", distance, number, 850,675, 50)\n  " +
            //"         parameter(\"\", \"bolt_shift_2\", distance, number, 850,720, 50)\n  " +
            ////pic
            //"  picture(\"bolt_anglepeb_plugin\", 0, 0, 913, 374)\n      " +







            "  }\n   " +




            " }\n}\n";



    }
}
