    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;


namespace Wpf3DChartTutorial
{
/// <summary>
/// This the main class that computes and displays the 3D chart in a WPF App
/// </summary>
    public class WPF3DChart: Control
    {
        #region Constants used in the class
        private const double    LITTLE_ABOVE                = 5.0;              // A constant to show the hit text a little above the bar
        private const int       MAX_INPUT_EXPECTED          = 1024;             // This is the maximum number of Inputs expected 
        private const string    DEFAULT_COLOR               = "DefaultColor";   // Default Color
        private Color           DEFAULT_COLOR_VALUE         = Colors.LightBlue; // Default Color, if the User has not specified any color
        private const double    PI_VALUE                    = 3.141;            // Math constant Pi value
        private const double    Z_ADJUST                    = -1.0;             // To calculate Z positions (being negative Z to be farthest.
        private const double    TEMP_POINT                  = 2.5;              // Temporary point just used to calculate the vector
        private const double    DEFAULT_BRUSH_OPACITY       = 0.985;            // Default brush opacity        
        private const int       DROP_COLOR_BRIGHTNESS       = 40;               // Used to reduce the color brightness
        private const int       DROP_MORE_COLOR_BRIGHTNESS  = 80;               // Used to reduce the color brightness
        private const double    DEFAULT_BAR_OPACITY         = 0.8;              // Default bar opacity
        private const double    SPACE_BETWEEN_BARS          = 2.0;              // Space between each bar in the grap
        private const double    CAMERA_FAR_PLANE_DISTANCE   = 10000.0;           // Camera's far plane distance
        private const double    CAMERA_NEAR_PLANE_DISTANCE  = 0.0;              // Camera's near plane distance
        private const double    CAMERA_FIELD_VIEW           = 45.0;             // Camera's field view
        private const double    DEFAULT_Z_CAMERA_POSITION   = 150.0;            // Default Camera position
        private const double    DEFAULT_CAMERA_DISTANCE     = 190.0;            // Default Camera distance
        private const double    DEFAULT_Z_CAMERA_ADJ        = 5.0;              // Value adj factor while increasing Z Items;
        private const double    DEFAULT_X_ANGLE             = -20;              // dEFAULT X Angle
        private const double    DEFAULT_Y_ANGLE             = -70;              // dEFAULT Y Angle
        private const double    DEFAULT_MOUSE_SENSITIVITY   = 4.0;                // Default Mouse sensitivity constant
        private const double    MAX_MOUSE_SENSITIVITY       = 20.0;               // Maximum Mouse sensitivity constant
        #endregion

        #region "Private Variables used for 3D Chart computation"

        private Window pThis;                           // This is the variable that stores the Main Window                   
        private bool leftButtonDown;                    // Holds the left mouse down button status
        private double MouseXFirstDown;                 // Holds the Mouse X position when the button was pressed
        private double MouseYFirstDown;                 // Holds the Mouse X position when the button was pressed

        private bool Initializing;                      // This variable is used to avoid Rendering while the chart is still initializing
        private Grid mainGrid;                          // The main grid on which the 3D Chart will be drawn.
        Viewport3D mainViewPort;                        // The main Viewport which will hold all the 3D Geometry drawings
        PerspectiveCamera persptCamera;                 // Camera used to see the scene
        
        private double XCameraPosition;                 // Used to hold X Camera position
        private double YCameraPosition;                 // Used to hold Y Camera position
        private double ZCameraPosition;                 // Used to hold Z Camera position
        private double cameraDistance;                  // Distance of Camera from  the scene
        private double centreX;                         // Centre X of the Drawing
        private double centreY;                         // Centre Y of the drawing 
        private double centreZ;                         // Centre Z of the drawing 
        private double XAngle;                          // X Angle at which the scene is perceived - In math terms theta
        private double YAngle;                          // Y angle at which the scene is preceived  - In math terms chi
        private Point3D cameraPosition;                 // 3D Camera position
        private Vector3D cameraLookDirection;           // Camera Look Direction vector
        private Model3DGroup lightsGroup;               // A Collection which holds a group of lights
        private ModelVisual3D lightModelVisual;

        private ModelVisual3D modelForHitText;          // Model used for Hit Test
        private GeometryModel3D geometryForHitText;     // Geomerty used for Hit test
        List<ModelVisual3D> modelsForHitTest;           // Models added during hit test
        private double OneLetterWidthForHitText;        // Width of the displayed text
        private double OneLetterHeightForHitText;       // Height of the displayed text
        private double LetterLengthForHitText;          // Length of the letter 
        private Size chartSize;                         // The length and the width of the Chart.

        private double xStartingPoint;                  // Used to calculate the X Starting point                      
        private List<double> xInGraph;                  // Used to calculate x points to draw the bars
        private List<double> zInGraph;                  // Used to calculate z points to draw the bars
        private double yInGraph;                        // Used to store Y Starting point

        private List<ModelVisual3D> modelArray;         // Used to store all the Geometry drawn for the 3D chart

        private Dictionary<GeometryModel3D, HitDetails> listOfHitPoints;    // Dictionary of Geometry mapped to Hit details
        private HitDetails SelectedHit = null;          // Used to store User Selected Hit point
        private HitDetails PrevSelectedHit = null;

        private Point3D PointToWrite0, PointToWrite1;   // Used to calculate the rectangle to write the text
        private Point3D PointToWrite2, PointToWrite3;   // Used to calculate the rectangle to write the text
        private TextBlock textToPrint;                  // Text block used write on the calculated rectangular space
        private MeshGeometry3D textDrawGeometry;        // Geomerty that contains the text
        private Vector3D textPlaneVector;               // Vector for the text
        private DiffuseMaterial textMaterial;           // Material used for writing the text
        private GeometryModel3D textGeometryModel;      // Geomerty model used for writing text
        private Point yPlane2DPoint0, yPlane2DPoint1;   // Used for drawing text
        private Point yPlane2DPoint2, yPlane2DPoint3;   // Used for drawing text

        #endregion 

        #region "These are related to WPF Dependency properties exposed to external world. i.e., users of this control"

        private string[] XItems;        // Array of X Items in the Bar chart.               
        private double[] YItems;        // Array of Y Items in the Bar chart
        private string[] ZItems;        // Array of Z Items in the Bar chart
        private Color[] ZPlaneColors;   // Array of Bar  Colors
        private Color XAxisColorItem;   // X Axis Color
        private Color YAxisColorItem;   // Y Axis Color

        /// <summary>
        /// The Below code is required for setting up WPF Dependency property.  This will help for Data Binding and other nice
        /// WPF facilities.
        /// </summary>
        /// 
        public static DependencyProperty HideChartProperty = DependencyProperty.Register("HideChart",
                                                                            typeof(bool),
                                                                            typeof(WPF3DChart),
                                                                            new FrameworkPropertyMetadata(false,
                                                                            FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                            new PropertyChangedCallback(OnHideChartChanged)));

        public static DependencyProperty MouseSensProperty = DependencyProperty.Register("MouseSens",
                                                                                    typeof(double),
                                                                                    typeof(WPF3DChart),
                                                                                    new FrameworkPropertyMetadata(0.0,
                                                                                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                    new PropertyChangedCallback(OnMouseValueChanged)));
                                                                                    
        public static DependencyProperty XValuesInputProperty = DependencyProperty.Register("XValuesInput", 
                                                                                            typeof(string), 
                                                                                            typeof(WPF3DChart), 
                                                                                            new FrameworkPropertyMetadata(  "",
                                                                                                                            FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                                                            new PropertyChangedCallback(OnXValueChanged)));
        public static DependencyProperty YValuesInputProperty = DependencyProperty.Register("YValuesInput", 
                                                                                            typeof(string), 
                                                                                            typeof(WPF3DChart),
                                                                                            new FrameworkPropertyMetadata(  "",
                                                                                                                            FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                                                            new PropertyChangedCallback(OnYValueChanged)));

        public static DependencyProperty ZValuesInputProperty = DependencyProperty.Register("ZValuesInput", 
                                                                                             typeof(string), 
                                                                                             typeof(WPF3DChart),
                                                                                             new FrameworkPropertyMetadata(  "",
                                                                                             FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                             new PropertyChangedCallback(OnZValueChanged)));


        public static DependencyProperty ChartTitleProperty = DependencyProperty.Register(  "ChartTitle",
                                                                                             typeof(string),
                                                                                             typeof(WPF3DChart),
                                                                                             new FrameworkPropertyMetadata("",
                                                                                             FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                             new PropertyChangedCallback(OnChartTitleChanged)));

        public static DependencyProperty ZValuesColorProperty = DependencyProperty.Register("ZValuesColor",
                                                                                             typeof(string),
                                                                                             typeof(WPF3DChart),
                                                                                             new FrameworkPropertyMetadata("",
                                                                                             FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                             new PropertyChangedCallback(OnZValuesColorChanged)));

        public static DependencyProperty YAxisColorProperty = DependencyProperty.Register(  "YAxisColor",
                                                                                            typeof(string),
                                                                                            typeof(WPF3DChart),
                                                                                            new FrameworkPropertyMetadata("",
                                                                                            FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                            new PropertyChangedCallback(OnYAxisColorChanged)));

        public static DependencyProperty XAxisColorProperty = DependencyProperty.Register(  "XAxisColor",
                                                                                            typeof(string),
                                                                                            typeof(WPF3DChart),
                                                                                            new FrameworkPropertyMetadata("",
                                                                                            FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                            new PropertyChangedCallback(OnXAxisColorChanged)));

        /// <summary>
        /// Gets or Sets the Hide Chart 
        /// </summary>
        public bool HideChart
        {
            get
            {
                return (bool)this.GetValue(HideChartProperty);
            }

            set
            {
                this.SetValue(HideChartProperty, value);
            }
        }


        /// <summary>
        /// Gets or Sets the Mouse sensitivity property.  
        /// </summary>
        public double MouseSens
        {
            get
            {
                return (double)this.GetValue(MouseSensProperty);
            }

            set
            {
                this.SetValue(MouseSensProperty, value);
            }
        }

        /// <summary>
        /// The below are the dependency properties exposed.  The only difference is, 
        /// this class do not store the values, but the base calss
        /// </summary>
        /// 

        /// <summary>
        /// Gets or Sets the X axis Color.  
        /// </summary>
        public string XAxisColor
        {
            get
            {
                return (string)this.GetValue(XAxisColorProperty);
            }

            set
            {
                this.SetValue(XAxisColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the Y axis Color.
        /// </summary>
        public string YAxisColor
        {
            get
            {
                return (string)this.GetValue(YAxisColorProperty);
            }

            set
            {
                this.SetValue(YAxisColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets X values input to the WPF 3D chart
        /// </summary>
        public string XValuesInput
        {
            get
            {
                return (string)this.GetValue(XValuesInputProperty);
            }

            set
            {
                this.SetValue(XValuesInputProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets Y values input to the WPF 3D chart
        /// </summary>
        public string YValuesInput
        {
            get
            {
                return (string)this.GetValue(YValuesInputProperty);
            }

            set
            {
                this.SetValue(YValuesInputProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets Z values to the WPF 3D chart
        /// </summary>
        public string ZValuesInput
        {
            get
            {
                return (string)this.GetValue(ZValuesInputProperty);
            }

            set
            {
                this.SetValue(ZValuesInputProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the WPF 3D Chart title
        /// </summary>
        public string ChartTitle
        {
            get
            {
                return (string)this.GetValue(ChartTitleProperty);
            }

            set
            {
                this.SetValue(ChartTitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the Bar colors for Z Items in WPF 3D bar chart
        /// </summary>
        public string ZValuesColor
        {
            get
            {
                return (string)this.GetValue(ZValuesColorProperty);
            }

            set
            {
                this.SetValue(ZValuesColorProperty, value);
            }
        }

        #endregion

        #region "The below methods are used for Computing the Array X,Y and Z values from the string input from user"
        /// <summary>
        /// Function that is called by the Call back function when the Property Hide Chart changes.
        /// </summary>
        /// <param name="currentObject"></param>
        private static void OnHideChartChanged(DependencyObject DependencyObjParam, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                WPF3DChart currentObject = (WPF3DChart)DependencyObjParam;
                if (currentObject == null) return;
                currentObject.Update();
            }
            catch
            {
                // If there's an exception, then exit gracefully showing error to the user
                MessageBox.Show("Error in HideChart values input.  Correct it and try again");
            }
        }

        /// <summary>
        /// Function that is called by the Call back function when the Property Bar Color / ZColorValues changes.
        /// This function converts the string input of Bar color values to an array of Color Values.
        /// </summary>
        /// <param name="currentObject"></param>
        private static void UpdateZColorValues(WPF3DChart currentObject)
        {
            try
            {
                // First let's split the Input Colors to string array.
                string[] ZValueColorStrings = currentObject.ZValuesColor.Split(",".ToCharArray(), MAX_INPUT_EXPECTED);

                // If there are no Z axis Items, then there is no point in computing the Colors.
                if (currentObject.ZItems == null || currentObject.ZItems.Length == 0) return;

                // Initialize an array of colors to hold the Z Values
                currentObject.ZPlaneColors = new Color[currentObject.ZItems.Length];

                int Counter = 0;
                // Fill in the Color values to the Array 
                foreach (string zValueColorItem in ZValueColorStrings)
                {
                    // Trim the string colors as user input may contain spaces
                    string trimmedzValueColorItem = zValueColorItem.Trim();
                    // This function ColorFromString returns Media Color from the string value.
                    currentObject.ZPlaneColors[Counter] = currentObject.ColorFromString(trimmedzValueColorItem);
                    Counter++;
                    // If the array is full, we need no more color values.
                    if (Counter >= currentObject.ZItems.Length) break;
                }

                // In case, if the user has not specified enough colors, we fill with default color
                while (Counter < currentObject.ZItems.Length)
                {
                    currentObject.ZPlaneColors[Counter] = currentObject.ColorFromString(DEFAULT_COLOR);
                    Counter++;
                }
            }
            catch
            {
                // If there's an exception, then exit gracefully showing error to the user
                MessageBox.Show("Error in Color values input.  Correct it and try again");
            }

        }


        /// <summary>
        /// Callback function when Mouse sensitivity value changed by the user or user program.
        /// </summary>
        /// <param name="DependencyObjParam"></param>
        /// <param name="e"></param>

        private static void OnMouseValueChanged(DependencyObject DependencyObjParam, DependencyPropertyChangedEventArgs e)
        {
            WPF3DChart currentObject = (WPF3DChart)DependencyObjParam;

            if (currentObject == null) return;

            try
            {
                currentObject.MouseSens = currentObject.MouseSens;
                if (currentObject.MouseSens < 0)
                {
                    currentObject.MouseSens = DEFAULT_MOUSE_SENSITIVITY;     // Default Mouse sensitivity constant
                }
                else if (currentObject.MouseSens > MAX_MOUSE_SENSITIVITY)
                {
                    currentObject.MouseSens = MAX_MOUSE_SENSITIVITY;               // Maximum Mouse sensitivity constant
                }
            }
            catch
            {
                currentObject.MouseSens = DEFAULT_MOUSE_SENSITIVITY;
            }

        }

        /// <summary>
        /// Callback function when Xaxis plane color value changed by the user or user program.
        /// </summary>
        /// <param name="DependencyObjParam"></param>
        /// <param name="e"></param>
        private static void OnXAxisColorChanged(DependencyObject DependencyObjParam, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                WPF3DChart currentObject = (WPF3DChart)DependencyObjParam;

                if (currentObject != null)
                {
                    // Simply call the ColorFromString to get the X plane color
                    currentObject.XAxisColorItem = currentObject.ColorFromString(currentObject.XAxisColor);
                }
            }
            catch
            {
                // If there's an exception, then exit gracefully showing error to the user
                MessageBox.Show("Error in X Axis Color value input.  Correct it and try again");
            }
        }

        /// <summary>
        /// Callback function called by the base class when YAxis plane color property value changes
        /// </summary>
        /// <param name="DependencyObjParam"></param>
        /// <param name="e"></param>
        private static void OnYAxisColorChanged(DependencyObject DependencyObjParam, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                WPF3DChart currentObject = (WPF3DChart)DependencyObjParam;

                if (null != currentObject)
                {
                    // Simply call the ColorFromString to get the X plane color
                    currentObject.YAxisColorItem = currentObject.ColorFromString(currentObject.YAxisColor);
                }
            }
            catch
            {
                // If there's an exception, then exit gracefully showing error to the user
                MessageBox.Show("Error in Y Axis Color value input.  Correct it and try again");
            }
        }

        /// <summary>
        /// Callback function that gets called when Z values color changes / i.e., Bar color changes
        /// </summary>
        /// <param name="DependencyObjParam"></param>
        /// <param name="e"></param>
        private static void OnZValuesColorChanged(DependencyObject DependencyObjParam, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                WPF3DChart currentObject = (WPF3DChart)DependencyObjParam;

                if (null != currentObject)
                {
                    // Updte the Bar color values by calling UpdateZColorValues
                    WPF3DChart.UpdateZColorValues(currentObject);
                    currentObject.Update();
                }
            }
            catch
            {
                // If there's an exception, then exit gracefully showing error to the user
                MessageBox.Show("Error in Bar Color values input.  Correct it and try again");
            }
        }

        /// <summary>
        /// Callback function that gets called when X value input for the 3D chart changes
        /// </summary>
        /// <param name="DependencyObjParam"></param>
        /// <param name="e"></param>
        private static void OnXValueChanged(DependencyObject DependencyObjParam, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                WPF3DChart currentObject = (WPF3DChart)DependencyObjParam;

                if (null != currentObject)
                {
                    // Fill in the XItems Array.
                    currentObject.XItems = currentObject.XValuesInput.Split(",".ToCharArray(), MAX_INPUT_EXPECTED);
                    // Update the Y values
                    UpdateYValues(currentObject);
                    // Update the graph
                    currentObject.Update();
                }
            }
            catch
            {
                // If there's an exception, then exit gracefully showing error to the user
                MessageBox.Show("Error in X values input.  Correct it and try again");
            }
        }

        /// <summary>
        /// This method is called by the callback function to update the Y values array in
        /// the 3D bar chart graph
        /// </summary>
        /// <param name="currentObject"></param>
        private static void UpdateYValues(WPF3DChart currentObject)
        {
            try
            {
                // Create a string array from the User input
                string[] YIemsArray = currentObject.YValuesInput.Split(",".ToCharArray(), MAX_INPUT_EXPECTED);

                // If there are no Z input values, there is  no point in computing the bar length using Y values. 
                if (currentObject.ZValuesInput == null || 
                    currentObject.XValuesInput == null ||
                    currentObject.ZValuesInput.Length * currentObject.XValuesInput.Length == 0) return;

                // Initialize the array with number of expected Y values.  This is because the user might have
                // given more values or less values.
                currentObject.YItems = new double[currentObject.ZValuesInput.Length * currentObject.XValuesInput.Length];
                int Counter = 0;

                // Fill in the array based  on user input
                foreach (string yItemInArray in YIemsArray)
                {
                    string yItem = yItemInArray.Trim();
                    try
                    {
                        currentObject.YItems[Counter] = double.Parse(yItem);
                    }
                    catch
                    {
                        // If the user input is wrong, fill that value to be 0.
                        currentObject.YItems[Counter] = double.Parse("0");
                    }
                    Counter++;
                }

                // If the user has less Y values inputs, then fill the empty with 0 values.
                while (Counter < currentObject.ZValuesInput.Length * currentObject.XValuesInput.Length)
                {
                    currentObject.YItems[Counter] = 0;
                    Counter++;
                }
            }
            catch
            {
                // If there's an exception, then exit gracefully showing error to the user
                MessageBox.Show("Error in Y values input.  Correct it and try again");
            }
        }

        /// <summary>
        /// Callback function that gets called when Y input values changed
        /// </summary>
        /// <param name="DependencyObjParam"></param>
        /// <param name="e"></param>
        private static void OnYValueChanged(DependencyObject DependencyObjParam, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                WPF3DChart currentObject = (WPF3DChart)DependencyObjParam;

                if (currentObject != null)
                {
                    WPF3DChart.UpdateYValues(currentObject);
                    currentObject.Update();
                }
            }
            catch
            {
                // If there's an exception, then exit gracefully showing error to the user
                MessageBox.Show("Error in Y values input.  Correct it and try again");
            }
        }

        /// <summary>
        /// Callback function that gets called when  Z values changes.
        /// </summary>
        /// <param name="DependencyObjParam"></param>
        /// <param name="e"></param>
        private static void OnZValueChanged(DependencyObject DependencyObjParam, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                WPF3DChart currentObject = (WPF3DChart)DependencyObjParam;

                if (currentObject != null)
                {
                    currentObject.ZItems = currentObject.ZValuesInput.Split(",".ToCharArray(), MAX_INPUT_EXPECTED);
                    // We need to update Y values, as the user might have not filled in Y values yet.
                    UpdateYValues(currentObject);
                    UpdateZColorValues(currentObject);
                    currentObject.Update();
                }
            }
            catch
            {
                // If there's an exception, then exit gracefully showing error to the user
                MessageBox.Show("Error in Z values input.  Correct it and try again");
            }
        }

        /// <summary>
        /// Callback function called when Chart title is changed.
        /// </summary>
        /// <param name="DependencyObjParam"></param>
        /// <param name="e"></param>
        private static void OnChartTitleChanged(DependencyObject DependencyObjParam, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                WPF3DChart currentObject = (WPF3DChart)DependencyObjParam;

                if (currentObject != null)
                {
                    // Update the  graph.
                    currentObject.Update();
                }
            }
            catch
            {
                // If there's an exception, then exit gracefully showing error to the user
                MessageBox.Show("Error in X values input.  Correct it and try again");
            }
        }
        #endregion

        #region "Just All ablut colors. This method is used for converting string Color (user Input) to Media Color"
        private Color ColorFromString(string stringColor)
        {
            stringColor = stringColor.Trim();
            Color retValue = DEFAULT_COLOR_VALUE;

            if (stringColor == DEFAULT_COLOR)
            {
                return retValue;
            }

            string colorTochk = stringColor.ToLower();

            #region A Big Switch case.. If at all you want to read this.
            switch (colorTochk )
            {
                case "darkblue":
                    retValue = Colors.DarkBlue;
                    break;
                case "aliceblue":
                    retValue = Colors.AliceBlue;
                    break;
                case "antiquewhite":
                    retValue = Colors.AntiqueWhite;
                    break;
                case "aqua":
                    retValue = Colors.Aqua;
                    break;
                case "aquamarine":
                    retValue = Colors.Aquamarine;
                    break;
                case "azure":
                    retValue = Colors.Azure;
                    break;
                case "beige":
                    retValue = Colors.Beige;
                    break;
                case "bisque":
                    retValue = Colors.Bisque;
                    break;
                case "black":
                    retValue = Colors.Black;
                    break;
                case "blanchedalmond":
                    retValue = Colors.BlanchedAlmond;
                    break;
                case "blue":
                    retValue = Colors.Blue;
                    break;
                case "blueviolet":
                    retValue = Colors.BlueViolet;
                    break;
                case "brown":
                    retValue = Colors.Brown;
                    break;
                case "burlywood":
                    retValue = Colors.BurlyWood;
                    break;
                case "cadetblue":
                    retValue = Colors.CadetBlue;
                    break;
                case "chartreuse":
                    retValue = Colors.Chartreuse;
                    break;
                case "chocolate":
                    retValue = Colors.Chocolate;
                    break;
                case "coral":
                    retValue = Colors.Coral;
                    break;
                case "papayawhip":
                    retValue = Colors.PapayaWhip;
                    break;
                case "cornflowerblue":
                    retValue = Colors.CornflowerBlue;
                    break;
                case "cornsilk":
                    retValue = Colors.Cornsilk;
                    break;
                case "crimson":
                    retValue = Colors.Crimson;
                    break;
                case "cyan":
                    retValue = Colors.Cyan;
                    break;
      
                case "darkcyan":
                    retValue = Colors.DarkCyan;
                    break;
                case "darkgoldenrod":
                    retValue = Colors.DarkGoldenrod;
                    break;
                case "darkgray":
                    retValue = Colors.DarkGray;
                    break;
                case "darkgreen":
                    retValue = Colors.DarkGreen;
                    break;
                case "darkkhaki":
                    retValue = Colors.DarkKhaki;
                    break;
                case "darkmagenta":
                    retValue = Colors.DarkMagenta;
                    break;
                case "darkolivegreen":
                    retValue = Colors.DarkOliveGreen;
                    break;
                case "darkorange":
                    retValue = Colors.DarkOrange;
                    break;
                case "darkred":
                    retValue = Colors.DarkRed;
                    break;
                case "darksalmon":
                    retValue = Colors.DarkSalmon;
                    break;
                case "darkseagreen":
                    retValue = Colors.DarkSeaGreen;
                    break;
                case "darkslateblue":
                    retValue = Colors.DarkSlateBlue;
                    break;

                case "darkslategray":
                    retValue = Colors.DarkSlateGray;
                    break;
                case "darkturquoise":
                    retValue = Colors.DarkTurquoise;
                    break;
                case "darkviolet":
                    retValue = Colors.DarkViolet;
                    break;
                case "deeppink":
                    retValue = Colors.DeepPink;
                    break;
                case "deepskyblue":
                    retValue = Colors.DeepSkyBlue;
                    break;
                case "dimgray":
                    retValue = Colors.DimGray;
                    break;
                case "dodgerblue":
                    retValue = Colors.DodgerBlue;
                    break;
                case "firebrick":
                    retValue = Colors.Firebrick;
                    break;
                case "floralwhite":
                    retValue = Colors.FloralWhite;
                    break;
                case "forestgreen":
                    retValue = Colors.ForestGreen;
                    break;
                case "fuchsia":
                    retValue = Colors.Fuchsia;
                    break;
                case "gainsboro":
                    retValue = Colors.Gainsboro;
                    break;
                case "ghostwhite":
                    retValue = Colors.GhostWhite;
                    break;
                case "gold":
                    retValue = Colors.Gold;
                    break;
                case "goldenrod":
                    retValue = Colors.Goldenrod;
                    break;
                case "gray":
                    retValue = Colors.Gray;
                    break;
                case "green":
                    retValue = Colors.Green;
                    break;
                case "greenyellow":
                    retValue = Colors.GreenYellow;
                    break;
                case "honeydew":
                    retValue = Colors.Honeydew;
                    break;
                case "hotpink":
                    retValue = Colors.HotPink;
                    break;
                case "indianred":
                    retValue = Colors.IndianRed;
                    break;
                case "indigo":
                    retValue = Colors.Indigo;
                    break;
                case "ivory":
                    retValue = Colors.Ivory;
                    break;
                case "khaki":
                    retValue = Colors.Khaki;
                    break;
                case "lavender":
                    retValue = Colors.Lavender;
                    break;
                case "lavenderblush":
                    retValue = Colors.LavenderBlush;
                    break;
                case "lawngreen":
                    retValue = Colors.LawnGreen;
                    break;
                case "lemonchiffon":
                    retValue = Colors.LemonChiffon;
                    break;
                case "lightblue":
                    retValue = Colors.LightBlue;
                    break;
                case "lightcoral":
                    retValue = Colors.LightCoral;
                    break;
                case "lightcyan":
                    retValue = Colors.LightCyan;
                    break;
                case "lightgoldenrodyellow":
                    retValue = Colors.LightGoldenrodYellow;
                    break;
                case "lightgray":
                    retValue = Colors.LightGray;
                    break;
                case "lightgreen":
                    retValue = Colors.LightGreen;
                    break;
                case "lightpink":
                    retValue = Colors.LightPink;
                    break;
                case "lightsalmon":
                    retValue = Colors.LightSalmon;
                    break;
                case "lightseagreen":
                    retValue = Colors.LightSeaGreen;
                    break;
                case "lightskyblue":
                    retValue = Colors.LightSkyBlue;
                    break;
                case "lightslategray":
                    retValue = Colors.LightSlateGray;
                    break;
                case "lightsteelblue":
                    retValue = Colors.LightSteelBlue;
                    break;
                case "lightyellow":
                    retValue = Colors.LightYellow;
                    break;
                case "lime":
                    retValue = Colors.Lime;
                    break;
                case "limegreen":
                    retValue = Colors.LimeGreen;
                    break;
                case "linen":
                    retValue = Colors.Linen;
                    break;
                case "magenta":
                    retValue = Colors.Magenta;
                    break;
                case "maroon":
                    retValue = Colors.Maroon;
                    break;
                case "mediumaquamarine":
                    retValue = Colors.MediumAquamarine;
                    break;
                case "mediumblue":
                    retValue = Colors.MediumBlue;
                    break;
                case "mediumorchid":
                    retValue = Colors.MediumOrchid;
                    break;
                case "mediumpurple":
                    retValue = Colors.MediumPurple;
                    break;
                case "mediumseagreen":
                    retValue = Colors.MediumSeaGreen;
                    break;
                case "mediumslateblue":
                    retValue = Colors.MediumSlateBlue;
                    break;
                case "mediumspringgreen":
                    retValue = Colors.MediumSpringGreen;
                    break;
                case "mediumturquoise":
                    retValue = Colors.MediumTurquoise;
                    break;
                case "mediumvioletred":
                    retValue = Colors.MediumVioletRed;
                    break;
                case "midnightblue":
                    retValue = Colors.MidnightBlue;
                    break;
                case "mintcream":
                    retValue = Colors.MintCream;
                    break;
                case "mistyrose":
                    retValue = Colors.MistyRose;
                    break;
                case "moccasin":
                    retValue = Colors.Moccasin;
                    break;
                case "navajowhite":
                    retValue = Colors.NavajoWhite;
                    break;

                case "navy":
                    retValue = Colors.Navy;
                    break;
                case "oldlace":
                    retValue = Colors.OldLace;
                    break;
                case "olive":
                    retValue = Colors.Olive;
                    break;
                case "olivedrab":
                    retValue = Colors.OliveDrab;
                    break;
                case "orange":
                    retValue = Colors.Orange;
                    break;
                case "orangered":
                    retValue = Colors.OrangeRed;
                    break;
                case "orchid":
                    retValue = Colors.Orchid;
                    break;
                case "palegoldenrod":
                    retValue = Colors.PaleGoldenrod;
                    break;
                case "palegreen":
                    retValue = Colors.PaleGreen;
                    break;
                case "paleturquoise":
                    retValue = Colors.PaleTurquoise;
                    break;
                case "palevioletred":
                    retValue = Colors.PaleVioletRed;
                    break;
                case "peachpuff":
                    retValue = Colors.PeachPuff;
                    break;
                case "peru":
                    retValue = Colors.Peru;
                    break;
                case "pink":
                    retValue = Colors.Pink;
                    break;
                case "plum":
                    retValue = Colors.Plum;
                    break;
                case "powderblue":
                    retValue = Colors.PowderBlue;
                    break;
                case "purple":
                    retValue = Colors.Purple;
                    break;
                case "red":
                    retValue = Colors.Red;
                    break;
                case "rosybrown":
                    retValue = Colors.RosyBrown;
                    break;
                case "royalblue":
                    retValue = Colors.RoyalBlue;
                    break;
                case "saddlebrown":
                    retValue = Colors.SaddleBrown;
                    break;
                case "salmon":
                    retValue = Colors.Salmon;
                    break;
                case "sandybrown":
                    retValue = Colors.SandyBrown;
                    break;
                case "seagreen":
                    retValue = Colors.SeaGreen;
                    break;
                case "seashell":
                    retValue = Colors.SeaShell;
                    break;
                case "sienna":
                    retValue = Colors.Sienna;
                    break;
                case "silver":
                    retValue = Colors.Silver;
                    break;
                case "skyblue":
                    retValue = Colors.SkyBlue;
                    break;
                case "slateblue":
                    retValue = Colors.SlateBlue;
                    break;
                case "slategray":
                    retValue = Colors.SlateGray;
                    break;
                case "snow":
                    retValue = Colors.Snow;
                    break;
                case "springgreen":
                    retValue = Colors.SpringGreen;
                    break;
                case "steelblue":
                    retValue = Colors.SteelBlue;
                    break;
                case "tan":
                    retValue = Colors.Tan;
                    break;
                case "teal":
                    retValue = Colors.Teal;
                    break;
                case "thistle":
                    retValue = Colors.Thistle;
                    break;
                case "tomato":
                    retValue = Colors.Tomato;
                    break;
                case "transparent":
                    retValue = Colors.Transparent;
                    break;
                case "turquoise":
                    retValue = Colors.Turquoise;
                    break;
                case "violet":
                    retValue = Colors.Violet;
                    break;
                case "wheat":
                    retValue = Colors.Wheat;
                    break;
                case "white":
                    retValue = Colors.White;
                    break;
                case "whitesmoke":
                    retValue = Colors.WhiteSmoke;
                    break;
                case "yellow":
                    retValue = Colors.Yellow;
                    break;
                case "yellowgreen":
                    retValue = Colors.YellowGreen;
                    break;

            }
            #endregion

            return retValue;
        }

        #endregion

        static WPF3DChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPF3DChart), new FrameworkPropertyMetadata(typeof(WPF3DChart)));
        }

        public WPF3DChart()
        {
        }

        #region "These are supportive methods used to draw the Geometry in the region"
        /// <summary>
        /// This function takes 3 parameters of a triangle.  Then calculates its normal
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {    
            Vector3D Vec0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D Vec1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(Vec0, Vec1);
        }

        /// <summary>
        /// This method is used to WriteText taking starting point, width, height and text
        /// to print as input
        /// </summary>
        /// <param name="StartPoint"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="TextToPrint"></param>
        /// <returns></returns>
        private GeometryModel3D WriteText(Point3D StartPoint, double Width, double Height, string TextToPrint, Color ColorToPrint)
        {
            //////////////////////////////////////////////////////////////////////////////////////////
            // The below blocks calculate rectangular co-ordinates inside which the text can be     //
            // written                                                                              //
            //////////////////////////////////////////////////////////////////////////////////////////
            PointToWrite0.X = StartPoint.X;
            PointToWrite0.Y = StartPoint.Y;
            PointToWrite0.Z = StartPoint.Z;

            PointToWrite1.X = StartPoint.X + Width;
            PointToWrite1.Y = StartPoint.Y;
            PointToWrite1.Z = StartPoint.Z;

            PointToWrite2.X = StartPoint.X + Width;
            PointToWrite2.Y = StartPoint.Y + Height;
            PointToWrite2.Z = StartPoint.Z;

            PointToWrite3.X = StartPoint.X;
            PointToWrite3.Y = StartPoint.Y + Height;
            PointToWrite3.Z = StartPoint.Z;
            //////////////////////////////////////////////////////////////////////////////////////////
            // The above blocks calculate rectangular co-ordinates inside which the text can be     //
            // written                                                                              //
            //////////////////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////////////////
            // The below blocks draws the rectangle.  The rectangle is formed using triangular      //
            // planes.                                                                              //
            //////////////////////////////////////////////////////////////////////////////////////////
            textDrawGeometry = new MeshGeometry3D();
            textDrawGeometry.Positions.Add(PointToWrite0);
            textDrawGeometry.Positions.Add(PointToWrite1);
            textDrawGeometry.Positions.Add(PointToWrite2);
            textDrawGeometry.Positions.Add(PointToWrite3);

            // Adding Indices for the triangles
            textDrawGeometry.TriangleIndices.Add(0);
            textDrawGeometry.TriangleIndices.Add(1);
            textDrawGeometry.TriangleIndices.Add(2);

            textDrawGeometry.TriangleIndices.Add(2);
            textDrawGeometry.TriangleIndices.Add(1);
            textDrawGeometry.TriangleIndices.Add(0);

            textDrawGeometry.TriangleIndices.Add(0);
            textDrawGeometry.TriangleIndices.Add(2);
            textDrawGeometry.TriangleIndices.Add(3);

            textDrawGeometry.TriangleIndices.Add(3);
            textDrawGeometry.TriangleIndices.Add(2);
            textDrawGeometry.TriangleIndices.Add(0);

            // Adding normals for the triangle
            textPlaneVector = CalculateNormal(PointToWrite2, PointToWrite1, PointToWrite0);
            textDrawGeometry.Normals.Add(textPlaneVector);
            textDrawGeometry.Normals.Add(textPlaneVector);
            textDrawGeometry.Normals.Add(textPlaneVector);

            textPlaneVector = CalculateNormal(PointToWrite0, PointToWrite1, PointToWrite2);
            textDrawGeometry.Normals.Add(textPlaneVector);
            textDrawGeometry.Normals.Add(textPlaneVector);
            textDrawGeometry.Normals.Add(textPlaneVector);

            textPlaneVector = CalculateNormal(PointToWrite3, PointToWrite2, PointToWrite0);
            textDrawGeometry.Normals.Add(textPlaneVector);
            textDrawGeometry.Normals.Add(textPlaneVector);
            textDrawGeometry.Normals.Add(textPlaneVector);

            textPlaneVector = CalculateNormal(PointToWrite0, PointToWrite2, PointToWrite3);
            textDrawGeometry.Normals.Add(textPlaneVector);
            textDrawGeometry.Normals.Add(textPlaneVector);
            textDrawGeometry.Normals.Add(textPlaneVector);

            // Ading Texture co-ordinates for the triangles.
            textDrawGeometry.TextureCoordinates.Add(yPlane2DPoint0);
            textDrawGeometry.TextureCoordinates.Add(yPlane2DPoint1);
            textDrawGeometry.TextureCoordinates.Add(yPlane2DPoint2);
            textDrawGeometry.TextureCoordinates.Add(yPlane2DPoint3);

            textToPrint = new TextBlock();
            textToPrint.Text = TextToPrint;
            textToPrint.Foreground = new SolidColorBrush(ColorToPrint);

            // We create a visual brush containing the text to print.
            textMaterial = new DiffuseMaterial(new VisualBrush(textToPrint));

            // Now this is returned to be added to the Viewport3D
            textGeometryModel = new GeometryModel3D(textDrawGeometry, textMaterial);

            return textGeometryModel;
        }

        /// <summary>
        /// This method is used to compute the camera position.  This is very useful while rotating the graph.
        /// </summary>
        private void ComputeCameraPosition()
        {
            /////////////////////////////////////////////////////////////////////////////////////////
            // The camera assumes a plane on a sphere.  Hence any point on sphere is calculated    //
            // when the user moves the mouse.  When we move the camera to this position the user   //
            // gets a view of the graph from that position in the sphere                           //
            /////////////////////////////////////////////////////////////////////////////////////////

            ZCameraPosition = centreZ + (Z_ADJUST) * 
                                        cameraDistance *
                                        Math.Cos(XAngle * PI_VALUE / 180.0) *
                                        Math.Sin(YAngle * PI_VALUE / 180.0);

            XCameraPosition = centreX + cameraDistance *
                                        Math.Sin(XAngle * PI_VALUE / 180.0) *
                                        Math.Sin(YAngle * PI_VALUE / 180.0);

            YCameraPosition = centreY + cameraDistance * Math.Cos(YAngle * PI_VALUE / 180.0);

            cameraPosition.X = XCameraPosition;
            cameraPosition.Y = YCameraPosition;
            cameraPosition.Z = ZCameraPosition;

            // We are using a constant TEMP_POINT  just to calculate the vector.
            cameraLookDirection.X = TEMP_POINT - XCameraPosition;
            cameraLookDirection.Y = TEMP_POINT - YCameraPosition;
            cameraLookDirection.Z = (Z_ADJUST * TEMP_POINT )- ZCameraPosition;

        }

        /// <summary>
        /// This method is used to calculate a rectangle taking input Points, Bar Color and Opacity.
        /// </summary>
        /// <param name="P0"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="P3"></param>
        /// <param name="BarColor"></param>
        /// <param name="OpacityIndex"></param>
        /// <returns></returns>
        private GeometryModel3D DrawRect(Point3D P0, Point3D P1, Point3D P2, Point3D P3, 
                                            Color BarColor, double OpacityIndex)
        {
            MeshGeometry3D side0Plane = new MeshGeometry3D();

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // Drawing two triangles for every rectangle that is to be formed.  Hence, we add positions,    //
            // triangle indices, and normals                                                                //
            //////////////////////////////////////////////////////////////////////////////////////////////////

            // Adding Positions
            side0Plane.Positions.Add(P0);
            side0Plane.Positions.Add(P1);
            side0Plane.Positions.Add(P2);
            side0Plane.Positions.Add(P3);

            // Adding triangle indices
            side0Plane.TriangleIndices.Add(0);
            side0Plane.TriangleIndices.Add(1);
            side0Plane.TriangleIndices.Add(2);

            side0Plane.TriangleIndices.Add(2);
            side0Plane.TriangleIndices.Add(1);
            side0Plane.TriangleIndices.Add(0);

            side0Plane.TriangleIndices.Add(0);
            side0Plane.TriangleIndices.Add(2);
            side0Plane.TriangleIndices.Add(3);

            side0Plane.TriangleIndices.Add(3);
            side0Plane.TriangleIndices.Add(2);
            side0Plane.TriangleIndices.Add(0);

            // Adding normals
            Vector3D normal = CalculateNormal(P2, P1, P0);
            side0Plane.Normals.Add(normal);
            side0Plane.Normals.Add(normal);
            side0Plane.Normals.Add(normal);

            normal = CalculateNormal(P0, P1, P2);
            side0Plane.Normals.Add(normal);
            side0Plane.Normals.Add(normal);
            side0Plane.Normals.Add(normal);

            normal = CalculateNormal(P3, P2, P0);
            side0Plane.Normals.Add(normal);
            side0Plane.Normals.Add(normal);
            side0Plane.Normals.Add(normal);

            normal = CalculateNormal(P0, P2, P3);
            side0Plane.Normals.Add(normal);
            side0Plane.Normals.Add(normal);
            side0Plane.Normals.Add(normal);

            // Atlast brush.
            MaterialGroup plane0MatGroup = new MaterialGroup();

            SolidColorBrush plane0Brush = new SolidColorBrush(BarColor);
            plane0Brush.Opacity = DEFAULT_BRUSH_OPACITY;
            plane0Brush.Opacity = OpacityIndex;
            DiffuseMaterial plane0Material = new DiffuseMaterial(plane0Brush);

            // Create the geometry to be added to the viewport
            plane0MatGroup.Children.Add(plane0Material);
            GeometryModel3D plane0Geometry = new GeometryModel3D(side0Plane, plane0MatGroup);

            return plane0Geometry;
        }

        /// <summary>
        /// This methods draws 3D bars getting X,Y,Z values, Point to start, height, width and BarColor.
        /// </summary>
        /// <param name="XItem"></param>
        /// <param name="YItem"></param>
        /// <param name="ZItem"></param>
        /// <param name="PointToStart"></param>
        /// <param name="Height"></param>
        /// <param name="Width"></param>
        /// <param name="BarColor"></param>
        /// <returns></returns>
        private List<ModelVisual3D> Draw3DBar(  string XItem, string YItem, string ZItem, Point3D PointToStart, double Height, 
                                                double Width, Color BarColor)
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            // This method first calculates the 8 points P0 to P7 required to draw the bar.  Then it calculates     //
            // the points to write the Hit Text.  I.e., when the bar is hit by a hovering mouse,  we need to        //
            // display the X,Y,Z values for that bar.  So this point P00 to P33 is calculated to draw the text      //
            // during hit text.  We store this information in a seperate class called HitDetails with the geometry  //
            // as the hit key. Then we draw the bars using the DrawRect method.                                     //
            //////////////////////////////////////////////////////////////////////////////////////////////////////////

            modelArray = new List<ModelVisual3D>();
            // Points are calculated to draw the bar
            Point3D P0 = new Point3D(PointToStart.X, PointToStart.Y, PointToStart.Z);
            Point3D P1 = new Point3D(PointToStart.X + Width, PointToStart.Y, PointToStart.Z);
            Point3D P2 = new Point3D(PointToStart.X + Width, PointToStart.Y, PointToStart.Z - Width);
            Point3D P3 = new Point3D(PointToStart.X, PointToStart.Y, PointToStart.Z - Width);

            Point3D P4 = new Point3D(PointToStart.X, PointToStart.Y + Height, PointToStart.Z);
            Point3D P5 = new Point3D(PointToStart.X + Width, PointToStart.Y + Height, PointToStart.Z);
            Point3D P6 = new Point3D(PointToStart.X + Width, PointToStart.Y + Height, PointToStart.Z - Width);
            Point3D P7 = new Point3D(PointToStart.X, PointToStart.Y + Height, PointToStart.Z - Width);
            ModelVisual3D myModelVisual = new ModelVisual3D();
            Model3DGroup myModelGroup = new Model3DGroup();

            // Points are calculated to draw the hit text and hit line
            Point3D P00 = new Point3D(xStartingPoint, P4.Y + 0.3, P4.Z - Width / 2.0);
            Point3D P11 = new Point3D(P4.X, P4.Y + 0.3, P4.Z - Width / 2.0);
            Point3D P22 = new Point3D(P4.X, P4.Y, P4.Z - Width / 2.0 - 0.3);
            Point3D P33 = new Point3D(xStartingPoint, P4.Y, P4.Z - Width / 2.0 - 0.3);
            Point3D PtToWrite = new Point3D(P4.X, P4.Y + LITTLE_ABOVE, P4.Z + Width / 2.0 - 0.3);

            HitDetails newHitDetails = new HitDetails(XItem, YItem, ZItem, P00, P11, P22, P33, PtToWrite, PointToStart, Width, Height, BarColor);
            
            GeometryModel3D rectVisual = DrawRect(P0, P1, P5, P4, BarColor, DEFAULT_BRUSH_OPACITY);
            listOfHitPoints.Add(rectVisual, newHitDetails);
            myModelGroup.Children.Add(rectVisual);

            // Lets draw the rectangles that form a full bar convering its top and bottom
            Color LightColor = BarColor;
            LightColor.A -= DROP_COLOR_BRIGHTNESS;
            rectVisual = DrawRect(P1, P2, P6, P5, LightColor, DEFAULT_BAR_OPACITY);
            listOfHitPoints.Add(rectVisual, newHitDetails);
            myModelGroup.Children.Add(rectVisual);

            rectVisual = DrawRect(P3, P2, P6, P7, BarColor, DEFAULT_BAR_OPACITY);
            listOfHitPoints.Add(rectVisual, newHitDetails);
            myModelGroup.Children.Add(rectVisual);

            rectVisual = DrawRect(P0, P3, P7, P4, LightColor, DEFAULT_BAR_OPACITY);
            listOfHitPoints.Add(rectVisual, newHitDetails);
            myModelGroup.Children.Add(rectVisual);
            LightColor.A -= DROP_MORE_COLOR_BRIGHTNESS;
            rectVisual = DrawRect(P4, P5, P6, P7, LightColor, DEFAULT_BRUSH_OPACITY);
            listOfHitPoints.Add(rectVisual, newHitDetails);
            myModelGroup.Children.Add(rectVisual);

            rectVisual = DrawRect(P0, P1, P2, P3, BarColor, DEFAULT_BRUSH_OPACITY);
            listOfHitPoints.Add(rectVisual, newHitDetails);
            myModelGroup.Children.Add(rectVisual);

            myModelVisual.Content = myModelGroup;
            modelArray.Add(myModelVisual);

            // return the model to be added to the main ViewPort3D
            return modelArray;
        }

        /// <summary>
        /// This method is used to compute the X,Y and Z bar starting positions to draw.
        /// </summary>
        /// <param name="NoOfXItems"></param>
        /// <param name="NoOfZItems"></param>
        /// <param name="BarWidth"></param>
        private void ComputeXZInGraph(int NoOfXItems, int NoOfZItems, double BarWidth)
        {
            double Width = chartSize.Width / 10.0;
            double Height = chartSize.Height / 10.0;
            NoOfXItems++;
            NoOfZItems++;

            // Compute the starting X position and One X unit with the available Width/height to the control
            double StartX = -1.0 * Width / 2.0 + 0.5;
            double StartY = -1.0 * Height / 2.0 + 0.5;
            double OneUnitX = (Width - 0.5) / NoOfXItems;
            double StartXPosition = StartX + OneUnitX;

            xStartingPoint = StartX;
            // Fill in an array of values where the bars will be placed
            yInGraph = StartY;
            for (int Counter = 0; Counter < NoOfXItems - 1; Counter++)
            {
                xInGraph.Add(StartXPosition);
                StartXPosition += OneUnitX;
            }

            double OneUnitZ = BarWidth * SPACE_BETWEEN_BARS;

            // Fill in an array of values of Z positions to draw the bar
            double StartZPosition = 0 - OneUnitZ;
            for (int Counter = 0; Counter < NoOfZItems - 1; Counter++)
            {
                zInGraph.Add(StartZPosition);
                StartZPosition -= OneUnitZ;
            }
        }

        /// <summary>
        /// This method is used to Draw the XZ and Y axis in the 3D bar chart
        /// </summary>
        /// <param name="XItems"></param>
        /// <param name="NumberOfXItems"></param>
        /// <param name="YIncrement"></param>
        /// <param name="NumberOfYItems"></param>
        /// <param name="ZItems"></param>
        /// <param name="NumberOfZItems"></param>
        /// <param name="BarWidth"></param>
        /// <param name="PlaneXColor"></param>
        /// <param name="PlaneYColor"></param>
        /// <param name="XMarkingColor"></param>
        /// <param name="YMarkingColor"></param>
        /// <param name="ZMarkingColor"></param>
        /// <returns></returns>
        private List<ModelVisual3D> DrawXYZWithMarkings( string[] XItems, int NumberOfXItems, double YIncrement, int NumberOfYItems, 
                                                        string[] ZItems, int NumberOfZItems, double BarWidth, Color PlaneXColor,
                                                        Color PlaneYColor, Color XMarkingColor, Color YMarkingColor, Color ZMarkingColor)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // This method draws the XZ and Y axis.  Not only that, this method draws the X, Y and Z markings.  These are important to   //
            // know the values of the bars to the user                                                                                   //
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            List<ModelVisual3D> listXYZ = new List<ModelVisual3D>();
            Model3DGroup myModelGroup = new Model3DGroup();
            ModelVisual3D myModelVisual = new ModelVisual3D();
            myModelVisual.Content = myModelGroup;
            listXYZ.Add(myModelVisual);

            NumberOfXItems++;
            NumberOfYItems++;
            NumberOfZItems++;

            xInGraph.Clear();
            zInGraph.Clear();
            yInGraph = 0;

            // We calculate One X,Y and Z units.  Also we calculate the starting points.
            double Width = chartSize.Width / 10.0;
            double Height = chartSize.Height / 10.0;

            double StartX = -1.0 * Width / 2.0 + 0.5;
            double StartY = -1.0 * Height / 2.0 + 0.5;
            double EndX = Width / 2.0;
            double EndY = -1.0 * Height / 2.0;
            double EndYY = Height / 2.0;

            xStartingPoint = StartX;

            double OneUnitX = (Width - 0.5) / NumberOfXItems;
            double OneUnitY = (Height - 0.5) / NumberOfYItems;
            double OneUnitZ = BarWidth * SPACE_BETWEEN_BARS;

            double ZEnd = (BarWidth * NumberOfZItems * (Z_ADJUST) * (SPACE_BETWEEN_BARS));

            centreX = Width / 2.0;
            centreY = Height / 2.0;
            centreZ = ZEnd / 2.0;

            //Compute the points to draw the XZ and Y axis. 
            Point3D PXpoint0 = new Point3D(StartX, StartY, 0);
            Point3D PXpoint1 = new Point3D(EndX, StartY, 0);
            Point3D PXpoint2 = new Point3D(EndX, StartY, ZEnd);
            Point3D PXpoint3 = new Point3D(StartX, StartY, ZEnd);

            // Draw the Plane XZ
            GeometryModel3D planeX = DrawRect(PXpoint3, PXpoint0, PXpoint1, PXpoint2, PlaneXColor, 0.9);

            Point3D PYpoint0 = new Point3D(StartX, StartY, 0);
            Point3D PYpoint1 = new Point3D(StartX, StartY, ZEnd);
            Point3D PYpoint2 = new Point3D(StartX, EndYY, ZEnd);
            Point3D PYpoint3 = new Point3D(StartX, EndYY, 0);


            Point3D PZpoint0 = new Point3D(StartX, StartY, ZEnd);
            Point3D PZpoint1 = new Point3D(EndX, StartY, ZEnd);
            Point3D PZpoint2 = new Point3D(EndX, EndYY, ZEnd);
            Point3D PZpoint3 = new Point3D(StartX, EndYY, ZEnd);

            Color PlaneZColor = PlaneYColor;
            PlaneZColor.A -= 120;

            myModelGroup.Children.Add(planeX);

            double MarkingWidth = 0.07;
            double MarkingHeight = 0.5;
            double OneLetterWidth = 0.7;
            double OneLetterHeight = 1.4;

            // Draw X markings.
            double StartXPosition = StartX + OneUnitX;
            yInGraph = StartY;
            for (int Counter = 0; Counter < NumberOfXItems - 1; Counter++)
            {
                xInGraph.Add(StartXPosition);
                Point3D pMarkX0 = new Point3D(StartXPosition - MarkingWidth, EndY + MarkingHeight, 0 - MarkingHeight);
                Point3D pMarkX1 = new Point3D(StartXPosition + MarkingWidth, EndY + MarkingHeight, 0 - MarkingHeight);
                Point3D PMarkX2 = new Point3D(StartXPosition + MarkingWidth, EndY, 0);
                Point3D pMarkX3 = new Point3D(StartXPosition - MarkingWidth, EndY, 0);

                Point3D pToWrite = new Point3D(StartXPosition - (XItems[Counter].Length * OneLetterWidth / 2.0), EndY + MarkingHeight, 0 - MarkingHeight);

                GeometryModel3D planeToMark = DrawRect(pMarkX0, pMarkX1, PMarkX2, pMarkX3, XMarkingColor, 0.985);
                myModelGroup.Children.Add(planeToMark);

                double LetterLength = XItems[Counter].Length * OneLetterWidth;
                planeToMark = WriteText(pToWrite, LetterLength, OneLetterHeight, XItems[Counter], Colors.Black);
                myModelGroup.Children.Add(planeToMark);

                StartXPosition += OneUnitX;
            }

            // Draw Y markings
            double PrevStartYPosition = StartY;
            double StartYPosition = StartY + OneUnitY;
            double StartYItem = YIncrement;
            bool FlipFlop = true;
            Color FlipColor = PlaneZColor;
            FlipColor.A -= 40;
            Color FlipYColor = PlaneYColor;
            FlipYColor.A -= 100;

            for (int Counter = 0; Counter <= NumberOfYItems - 1; Counter++)
            {
                Point3D pMarkY0 = new Point3D(StartX - MarkingHeight / 2.0, StartYPosition - MarkingWidth, 0);
                Point3D pMarkY1 = new Point3D(StartX - MarkingHeight / 2.0, StartYPosition + MarkingWidth, 0);
                Point3D PMarkY2 = new Point3D(StartX + MarkingHeight / 2.0, StartYPosition + MarkingWidth, 0 - MarkingHeight);
                Point3D pMarkY3 = new Point3D(StartX + MarkingHeight / 2.0, StartYPosition - MarkingWidth, 0 - MarkingHeight);

                GeometryModel3D planeToMark = DrawRect(pMarkY0, pMarkY1, PMarkY2, pMarkY3, YMarkingColor, 0.985);
                myModelGroup.Children.Add(planeToMark);

                StringBuilder YItemToWrite = new StringBuilder();
                YItemToWrite.AppendFormat("{0,2:f}", StartYItem);
                double LetterLength = YItemToWrite.Length * OneLetterWidth;

                Point3D pToWrite = new Point3D(StartX - MarkingHeight / 2.0 - LetterLength, StartYPosition - MarkingWidth, 0);
                planeToMark = WriteText(pToWrite, LetterLength, OneLetterHeight, YItemToWrite.ToString(), Colors.Black);
                myModelGroup.Children.Add(planeToMark);

                Point3D TempPYpoint0 = new Point3D(StartX, PrevStartYPosition, ZEnd);
                Point3D TempPYpoint1 = new Point3D(EndX, PrevStartYPosition, ZEnd);
                Point3D TempPYpoint2 = new Point3D(EndX, StartYPosition, ZEnd);
                Point3D TempPYpoint3 = new Point3D(StartX, StartYPosition, ZEnd);

                Point3D TempPYpoint00 = new Point3D(StartX, PrevStartYPosition, 0);
                Point3D TempPYpoint11 = new Point3D(StartX, PrevStartYPosition, ZEnd);
                Point3D TempPYpoint22 = new Point3D(StartX, StartYPosition, ZEnd);
                Point3D TempPYpoint33 = new Point3D(StartX, StartYPosition, 0);
               
                // Draw the Plane Y
                if (FlipFlop == true)
                {
                    GeometryModel3D planeYTemp = DrawRect(TempPYpoint0, TempPYpoint1, TempPYpoint2, TempPYpoint3, FlipYColor, 0.9);
                    myModelGroup.Children.Add(planeYTemp);
                    planeYTemp = DrawRect(TempPYpoint00, TempPYpoint11, TempPYpoint22, TempPYpoint33,PlaneYColor , 0.9);
                    myModelGroup.Children.Add(planeYTemp);
                    FlipFlop = false;
                }
                else
                {
                    GeometryModel3D planeYTemp = DrawRect(TempPYpoint0, TempPYpoint1, TempPYpoint2, TempPYpoint3, PlaneYColor, 0.9);
                    myModelGroup.Children.Add(planeYTemp);
                    planeYTemp = DrawRect(TempPYpoint00, TempPYpoint11, TempPYpoint22, TempPYpoint33, FlipYColor, 0.9);
                    myModelGroup.Children.Add(planeYTemp);
                    FlipFlop = true;
                }
                PrevStartYPosition = StartYPosition;


                StartYPosition += OneUnitY;
                StartYItem += YIncrement;
            }

            // Draw Z markings
            double StartZPosition = 0 - OneUnitZ;
            for (int Counter = 0; Counter < NumberOfZItems - 1; Counter++)
            {
                zInGraph.Add(StartZPosition);
                Point3D pMarkX0 = new Point3D(EndX - MarkingHeight / 2.0, EndY, StartZPosition + MarkingWidth);
                Point3D pMarkX1 = new Point3D(EndX + MarkingHeight / 2.0, EndY + MarkingHeight / 2.0, StartZPosition + MarkingWidth);
                Point3D PMarkX2 = new Point3D(EndX + MarkingHeight / 2.0, EndY + MarkingHeight / 2.0, StartZPosition - MarkingWidth);
                Point3D pMarkX3 = new Point3D(EndX - MarkingHeight / 2.0, EndY, StartZPosition - MarkingWidth);

                Point3D pToWrite = new Point3D(EndX - MarkingHeight / 2.0 + 1.0, EndY, StartZPosition + MarkingWidth);


                GeometryModel3D planeToMark = DrawRect(pMarkX0, pMarkX1, PMarkX2, pMarkX3, XMarkingColor, 0.985);
                myModelGroup.Children.Add(planeToMark);

                double LetterLength = ZItems[Counter].Length * OneLetterWidth;
                planeToMark = WriteText(pToWrite, LetterLength, OneLetterHeight, ZItems[Counter], Colors.Black);
                myModelGroup.Children.Add(planeToMark);

                StartZPosition -= OneUnitZ;
            }

            if (OneLetterWidth * 2.0 * ChartTitle.Length > chartSize.Width/10.0)
            {
                double Length = chartSize.Width / (OneLetterWidth * 20.0);
                string strTitle = ChartTitle.Substring(0, (int)Length - 3);
                strTitle += "...";
                GeometryModel3D titleToDisplay = WriteText(new Point3D((chartSize.Width / -20.0), (chartSize.Height / -20.0) - 7.0, 2.5), OneLetterWidth * 2.0 * strTitle.Length, OneLetterHeight * 2.0, strTitle, Colors.Black);
                myModelGroup.Children.Add(titleToDisplay);

            }
            else
            {
                GeometryModel3D titleToDisplay = WriteText(new Point3D((chartSize.Width / -20.0), (chartSize.Height / -20.0) - 7.0, 2.5), OneLetterWidth * 2.0 * ChartTitle.Length, OneLetterHeight * 2.0, ChartTitle, Colors.Black);
                myModelGroup.Children.Add(titleToDisplay);
            }

            return listXYZ;
        }

        #endregion

        #region "The below are Mouse methods used for spinning/rotating the geometry based on user's mouse move

        /// <summary>
        /// This method is called when the mouse is moved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="me"></param>
        private void OnMouseMove(object sender, MouseEventArgs me)
        {
            if (me.LeftButton == MouseButtonState.Pressed && leftButtonDown == true)
            {
                // If the left mouse button is pressed, then we calculate the Camera position, 
                // then invalidate the current drawing for a redraw
                me.MouseDevice.SetCursor(Cursors.ScrollAll);
                Point retPoint = me.GetPosition(pThis);
                double MouseX = retPoint.X;
                double MouseY = retPoint.Y;

                if (MouseX == MouseXFirstDown && MouseY == MouseYFirstDown)
                {
                    return;
                }

                // To calculate how much the mouse moved X position
                if (MouseXFirstDown != MouseX)
                {
                    XAngle += (MouseX - MouseXFirstDown) / MouseSens;
                    MouseXFirstDown = MouseX;
                }

                // To calculate how much the mouse mofed Y position
                if (MouseYFirstDown != MouseY)
                {
                    YAngle += (MouseY - MouseYFirstDown) / MouseSens;
                    MouseYFirstDown = MouseY;
                }

                //ZCameraPosition = -2.5 + (-1) * cameraDistance * Math.Cos(XAngle * 3.141 / 180.0) * Math.Sin(YAngle * 3.141 / 180.0);
                //XCameraPosition = 2.5 + cameraDistance * Math.Sin(XAngle * 3.141 / 180.0) * Math.Sin(YAngle * 3.141 / 180.0);
                //YCameraPosition = 2.5 + cameraDistance * Math.Cos(YAngle * 3.141 / 180.0);
                ComputeCameraPosition();
                SelectedHit = null;
            }
            else
            {
                Point pt = me.GetPosition((UIElement)sender);
                HitTestResult result = VisualTreeHelper.HitTest(mainGrid, pt);

                // If there is a hit, then draw the bar slightly bigger and display the Hit text 
                // (as we stored thia already while drawing
                if (result != null)
                {
                    RayHitTestResult res = result as RayHitTestResult;
                    if (res != null)
                    {
                        GeometryModel3D geoMod = res.ModelHit as GeometryModel3D;

                        HitDetails myHitDetails;
                        if (listOfHitPoints.TryGetValue(geoMod, out myHitDetails))
                        {
                            me.MouseDevice.SetCursor(Cursors.Hand);
                            SelectedHit = myHitDetails;
                            SelectedHit.ShowValue = false;
                            this.InvalidateVisual();
                            return;
                        }
                    }
                }

                SelectedHit = null;
            }
            this.InvalidateVisual();
        }

        /// <summary>
        /// This methodis called when the mouse button is down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            // If the left mouse button is down, we compute the mouse first pressed point
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                e.MouseDevice.SetCursor(Cursors.ScrollAll);
                leftButtonDown = true;
                Point retPoint = e.GetPosition(pThis);
                MouseXFirstDown = retPoint.X;
                MouseYFirstDown = retPoint.Y;
            }
            else
            {
                leftButtonDown = false;
            }

            
        }

       

        /// <summary>
        /// This method is called when Left mouse button is up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseLeftButtonUpEventHandler(object sender, MouseEventArgs e)
        {
            // We reset the boolean so  that we dont move the graph when the user is not 
            // pressing the left mouse down
            leftButtonDown = false;
            
        }

        #endregion

        #region "This method is the main method that draws the 3D bar chart "
        /// <summary>
        /// This method draws the 3D bar chart.
        /// </summary>
        /// <param name="XItems"></param>
        /// <param name="YItems"></param>
        /// <param name="ZItems"></param>
        /// <param name="xStartingPointColor"></param>
        /// <param name="YPlaneColor"></param>
        /// <param name="ZPlaneColor"></param>
        /// <param name="BarWidth"></param>
        /// <returns></returns>
        private List<ModelVisual3D> Draw3DChart(string[] XItems, double[] YItems, string[] ZItems,
                                Color xStartingPointColor, Color YPlaneColor, Color[] ZPlaneColor, double BarWidth)
        {
            List<ModelVisual3D> modelsToAdd = new List<ModelVisual3D>();

            // This If block is just for understanding sake.  The compiler will remove this during release build.
            if (XItems.Length * ZItems.Length != YItems.Length || ZPlaneColor.Length <= ZItems.Length)
            {
                // We will display less items in the graph then..
            }

            // If there are anything to draw then enter in this if block
            if (YItems.Length > 0 && XItems.Length > 0 && ZItems.Length > 0)
            {
                // Calculate the max Y point
                double MaxY = YItems[0];

                for (int Counter = 1; Counter < XItems.Length * ZItems.Length; Counter++)
                {
                    if (MaxY < YItems[Counter])
                    {
                        MaxY = YItems[Counter];
                    }
                }

                // Calculate the Height of the longest bar and also calculate On Y Unit
                double Ht = chartSize.Height / 30.0;

                double OneYUnit = ((chartSize.Height / 10.0)) / MaxY;

                ComputeXZInGraph(XItems.Length, YItems.Length, BarWidth);

                OneYUnit = ((chartSize.Height / 10.0) - (chartSize.Height / (10 * Ht))) / MaxY;
                int CounterY = 0;

                // Draw the Bars one by one based on values computed in the method ComputeXZInGraph above
                for (int CounterZ = 0; CounterZ < ZItems.Length; CounterZ++)
                {
                    for (int CounterX = 0; CounterX < XItems.Length; CounterX++)
                    {
                        List<ModelVisual3D> modelBars = Draw3DBar(XItems[CounterX], YItems[CounterY].ToString(), ZItems[CounterZ], new Point3D(xInGraph[CounterX] - BarWidth / 2.0, yInGraph, zInGraph[CounterZ] + BarWidth / 2.0),
                                                                OneYUnit * YItems[CounterY], BarWidth, ZPlaneColor[CounterZ]);
                        List<ModelVisual3D>.Enumerator enumModels = modelBars.GetEnumerator();

                        while (enumModels.MoveNext())
                        {
                            modelsToAdd.Add(enumModels.Current);
                        }
                        CounterY++;
                    }
                }

                //// Now draw the XZ plane and Y plane
                Ht = (chartSize.Height /50.0);

                OneYUnit = Ht / MaxY;
                OneYUnit = 1 / OneYUnit;             
//                List<ModelVisual3D> modelBars1 = DrawXYZWithMarkings(XItems, XItems.Length, MaxY / (Ht - 1), (int)Ht, ZItems, ZItems.Length, BarWidth, XAxisColorItem,
  //                                          YAxisColorItem, Colors.Black, Colors.Black, Colors.Black);
                List<ModelVisual3D> modelBars1 = DrawXYZWithMarkings(XItems, XItems.Length, OneYUnit, (int)(Ht), ZItems, ZItems.Length, BarWidth, XAxisColorItem,
                                          YAxisColorItem, Colors.Black, Colors.Black, Colors.Black);

                List<ModelVisual3D>.Enumerator enumModels1 = modelBars1.GetEnumerator();

                while (enumModels1.MoveNext())
                {
                    modelsToAdd.Add(enumModels1.Current);
                }

            }

            // Return the model so that it can be added to the main Viewport3D
            return modelsToAdd;
        }
        #endregion

        #region "These are overridden methods of the Base class Control"

        /// <summary>
        /// This is the method that gets to know the size available for the 3D chart.  This value is stored
        /// which will be used in the Initialize method.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            chartSize.Height = constraint.Height;
            chartSize.Width = constraint.Width;
            Initialize();


            return base.MeasureOverride(constraint);
        }


        /// <summary>
        /// This method is used to initialize the values and also attach ourselves to the main window of the 
        /// application.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            mainGrid = new Grid();
            mainViewPort = new Viewport3D();
            
            // Attach our control to the main window
            //pThis = Application.Current.MainWindow;
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                pThis = Window.GetWindow(this);
            }
            else 
            {
                pThis = Application.Current.MainWindow;
            }
            UIElement otherUIElements = pThis.Content as UIElement;
            pThis.Content = null;
            mainGrid.Children.Add(mainViewPort);
            mainGrid.Children.Add(otherUIElements);
            pThis.Content = mainGrid;

            // Add a camera to the scene
            cameraPosition = new Point3D(XCameraPosition, YCameraPosition, ZCameraPosition);
            cameraLookDirection = new Vector3D( TEMP_POINT - XCameraPosition, 
                                                TEMP_POINT - YCameraPosition, 
                                                ((Z_ADJUST) * (TEMP_POINT)) - ZCameraPosition);

            persptCamera = new PerspectiveCamera(cameraPosition, cameraLookDirection, new Vector3D(0, 1, 0), 200);


            persptCamera.FarPlaneDistance = CAMERA_FAR_PLANE_DISTANCE;
            persptCamera.LookDirection = cameraLookDirection;
            persptCamera.UpDirection = new Vector3D(0, 1, 0);
            persptCamera.NearPlaneDistance = CAMERA_NEAR_PLANE_DISTANCE;
            persptCamera.Position = cameraPosition;
            persptCamera.FieldOfView = CAMERA_FIELD_VIEW;
            mainViewPort.Camera = persptCamera;

            
            // Add lights to the scene
            lightModelVisual = new ModelVisual3D();

            lightsGroup = new Model3DGroup();
            DirectionalLight light2 = new DirectionalLight(Colors.White, new Vector3D(0, 0, 1));
            lightsGroup.Children.Add(light2);
            DirectionalLight light1 = new DirectionalLight(Colors.White, new Vector3D(1, 0, 0));
            lightsGroup.Children.Add(light1);
            DirectionalLight light0 = new DirectionalLight(Colors.White, new Vector3D(-1, 0, 0));
            lightsGroup.Children.Add(light0);
            DirectionalLight light3 = new DirectionalLight(Colors.White, new Vector3D(0, 0, -1));
            lightsGroup.Children.Add(light3);
            DirectionalLight light4 = new DirectionalLight(Colors.White, new Vector3D(0, 1, 0));
            lightsGroup.Children.Add(light4);
            DirectionalLight light5 = new DirectionalLight(Colors.White, new Vector3D(0, -1, 0));
            lightsGroup.Children.Add(light5);
            lightModelVisual.Content = lightsGroup;

            // Add mouse movements
            mainGrid.MouseLeftButtonUp += new MouseButtonEventHandler(MouseLeftButtonUpEventHandler);
            mainGrid.MouseMove += new MouseEventHandler(OnMouseMove);
            mainGrid.MouseDown += new MouseButtonEventHandler(OnMouseDown);
         
            listOfHitPoints = new Dictionary<GeometryModel3D, HitDetails>();

            // Give default values to the properties and other stuff

            XAngle = DEFAULT_X_ANGLE;
            YAngle = DEFAULT_Y_ANGLE;

            ChartTitle = "Параметрична схема ситуації";
            XAxisColor = "Bisque";
            YAxisColor = "Burlywood";
            XValuesInput = "За Мир, Злагода, Стабільність, Ділом, Аграрна партія"; //дії
            YValuesInput = "1, 1, 2, 2, 0, 2, 2, 2, 1, 0, 0, 2, 2, 2, 1, 0, 0, 1, 1, 2"; //наслідки //мають бути цифри!
            ZValuesInput = "Правлячі, Новоутворені, Маргінальні, Опозиційні"; //значення неспостережуваного параметру        
            ZValuesColor = "DarkGreen, DarkBlue, DarkRed, DarkCyan";
            MouseSens = DEFAULT_MOUSE_SENSITIVITY;

            XCameraPosition = centreX;
            YCameraPosition = centreY;
            ZCameraPosition = DEFAULT_Z_CAMERA_POSITION;
            cameraDistance = DEFAULT_CAMERA_DISTANCE;
            cameraDistance = 175 + DEFAULT_Z_CAMERA_ADJ * ZItems.Length;

            xInGraph = new List<double>();
            zInGraph = new List<double>();

            base.OnInitialized(e);

        }

        /// <summary>
        /// This medhod is used to change the chart behaviour from design time to runtime.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                // At runtime we just minimize this Template Image
                Image thisImage = this.Template.FindName("TemplateImage", this) as Image;
                thisImage.Width = 0;
                thisImage.Height = 0;
            }


            base.OnApplyTemplate();
        }

        /// <summary>
        /// This method is called when rendering is required.  
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!Initializing)
            {
                // Update the CAmera position
                persptCamera.LookDirection = cameraLookDirection;
                persptCamera.Position = cameraPosition;

                // If the user's mouse is on one of the 3D bar. then enhance the bar and display its values.
                if (null != SelectedHit && 
                    (PrevSelectedHit == null || 
                    0 != PrevSelectedHit.StringToDisplay.CompareTo(SelectedHit.StringToDisplay)))
                {
                    if (geometryForHitText != null)
                    {
                        geometryForHitText = null;
                        modelForHitText.Content = null;
                        mainViewPort.Children.Remove(modelForHitText);
                    }

                    if (modelsForHitTest != null)
                    {
                        List<ModelVisual3D>.Enumerator enumModelsI = modelsForHitTest.GetEnumerator();

                        while (enumModelsI.MoveNext())
                        {
                            mainViewPort.Children.Remove(enumModelsI.Current);
                        }
                    }

                    LetterLengthForHitText = SelectedHit.StringToDisplay.Length * OneLetterWidthForHitText;
                    Point3D ptToWRite = SelectedHit.P2;
                    ptToWRite.Y += LITTLE_ABOVE;
                    
                    geometryForHitText = WriteText(SelectedHit.PointToWrite, LetterLengthForHitText, OneLetterHeightForHitText, SelectedHit.StringToDisplay, Colors.Black);

                    modelsForHitTest = Draw3DBar(SelectedHit.XItem, SelectedHit.YItem, SelectedHit.ZItem, SelectedHit.PointToStart, SelectedHit.Height, SelectedHit.Width , SelectedHit.BarColor);

                    List<ModelVisual3D>.Enumerator enumModels = modelsForHitTest.GetEnumerator();

                    while (enumModels.MoveNext())
                    {
                        mainViewPort.Children.Add(enumModels.Current);
                    }
                    
                    PrevSelectedHit = SelectedHit;

                    modelForHitText.Content = geometryForHitText;
                    mainViewPort.Children.Add(modelForHitText);
                }
                    // Else if there is no bar selected remove the enhancement and Hit text.
                else if (null == SelectedHit)
                {
                    geometryForHitText = null;
                    modelForHitText.Content = null;
                    mainViewPort.Children.Remove(modelForHitText);

                    if (modelsForHitTest != null)
                    {
                        List<ModelVisual3D>.Enumerator enumModels = modelsForHitTest.GetEnumerator();

                        while (enumModels.MoveNext())
                        {
                            mainViewPort.Children.Remove(enumModels.Current);
                        }
                    }
                }

            }
            
 	        base.OnRender(drawingContext);
        }
        #endregion

        /// <summary>
        /// This method is called to update the graph after user input
        /// </summary>
        public void Update()
        {
            Initialize();
        }

        #region "This is the method used/called during initialization.  Also this is called to update the Graph with new values"
        /// <summary>
        /// This method is called during initialization to create the 3D Bar chart and then its called to update
        /// </summary>
        private void Initialize()
        {
            if (chartSize.Height == 0 || chartSize.Width == 0) return;
            Initializing = true;
            cameraDistance = 160 + 5 * ZItems.Length + 140 / XItems.Length;
            

            // We clear the 3D drawings, if any previously added
            mainViewPort.Children.Clear();
            if (HideChart == true) return;
            mainViewPort.Children.Add(lightModelVisual);

            listOfHitPoints.Clear();
            this.InvalidateVisual();

            List<ModelVisual3D> retValue;
            List<ModelVisual3D>.Enumerator enumList;

            SelectedHit = null;
            modelForHitText = new ModelVisual3D();
            OneLetterWidthForHitText = 1.1;
            OneLetterHeightForHitText = 1.7;

            PointToWrite0 = new Point3D();
            PointToWrite1 = new Point3D();
            PointToWrite2 = new Point3D();
            PointToWrite3 = new Point3D();

            yPlane2DPoint0 = new Point(0, 1);
            yPlane2DPoint1 = new Point(1, 1);
            yPlane2DPoint2 = new Point(1, 0);
            yPlane2DPoint3 = new Point(0, 0);

            // We draw 3D cart here.
            retValue = Draw3DChart(XItems, YItems, ZItems, Colors.Yellow, Colors.Cyan, ZPlaneColors, chartSize.Width/(XItems.Length * 2.2 * 10));
            enumList = retValue.GetEnumerator();

            // Add the resultant geometry to the main viewport.
            while (enumList.MoveNext())
            {
                mainViewPort.Children.Add(enumList.Current);
            }

            retValue.Clear();
            // Compute the camera position.
            ComputeCameraPosition();
            Initializing = false;

            // Display the graph
            this.InvalidateVisual();
        }
        #endregion
    }
}
