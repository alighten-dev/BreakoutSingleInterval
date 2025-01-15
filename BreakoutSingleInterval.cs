#region Using declarations
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.NinjaScript.DrawingTools;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
    public class BreakoutSingleInterval : Indicator
    {
        public partial class Breakout
        {
            public Breakout(int i, bool enable, int time, double h, double l, Brush _brush) //, string t
            {
                index = i;
                enable = enabled;
                high = h;
                low = l;
                b_high = false;
                b_low = false;
                brush = _brush;

            }


            public BarsPeriodType type { get; set; }
            public int index { get; set; }
            public int barIndex { get; set; }
            public int patternIndex { get; set; }
            public bool enabled { get; set; }
            public bool b_high { get; set; }
            public bool b_low { get; set; }
            public int time { get; set; }
            public double high { get; set; }
            public double low { get; set; }
            public Brush brush { get; set; }

            public string _type { get; set; }


        }
        [XmlIgnore]
		private Brush brush1;
        private int index;
		public Breakout SingleBreakout;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"All credit goes to rlopjor26/flexusFuture for his much better indicator https://ninjatraderecosystem.com/user-app-share-download/breakouts-multi-interval/";
                Name = "Breakout Single-Interval";
                Calculate = Calculate.OnBarClose;
                IsOverlay = true;
                DisplayInDataBox = true;
                DrawOnPricePanel = true;
                DrawHorizontalGridLines = true;
                DrawVerticalGridLines = true;
                PaintPriceMarkers = true;
                BarsRequiredToPlot = 5;
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                IsSuspendedWhileInactive = true;
                IsAutoScale = false;
                TF1Type = BarsPeriodType.Minute;
             
                enable_timeframe1 = true;
            
                timeframe1_value = 60;

                color1 = Brushes.Crimson;

                len = 50000;
                width = 3;
            }
            else if (State == State.Configure)
            {				
               	BarsPeriod barPeriod = new BarsPeriod { BarsPeriodType = TF1Type, Value = timeframe1_value };
               	AddDataSeries(this.Instrument.FullName, barPeriod, 1000, "", true);		
            }
			else if (State == State.DataLoaded)
			{
				brush1 = color1;
				SingleBreakout = new Breakout(1,true,0,0,0, brush1);
				
				SingleBreakout.type = TF1Type;
                SingleBreakout.index = 1;
                SingleBreakout.time = timeframe1_value;
                SingleBreakout.enabled = true;
			}
            else if (State == State.Historical)
            {
                //SetZOrder(-1);
            }
        }

        protected override void OnBarUpdate()
        {

			if (CurrentBars[0] < BarsRequiredToPlot)
            {
                return;
            }
			if (CurrentBars[1] < BarsRequiredToPlot)
            {
                return;
            }
      
            if (BarsInProgress == 1)
            {
                index = BarsInProgress;
               	FindEngulf();
                FindMother();

            }

            if (BarsInProgress == 0)
            {

                UpdateBreakouts();
            }
        }
        

        private void UpdateBreakouts()
        {
           
            if (SingleBreakout.barIndex != 0)
            {
                if (!SingleBreakout.b_high && High[0] > SingleBreakout.high)
                {

                    SingleBreakout.b_high = true;
                }
                else if (!SingleBreakout.b_low && Low[0] < SingleBreakout.low)
                {

                    SingleBreakout.b_low = true;
                }
            }

        
        	RemoveDrawings();

        }
        private void RemoveDrawings()
        {
           
            if (SingleBreakout.b_high && SingleBreakout.b_low)
            {
                RemoveDrawObject("high_" + SingleBreakout.time + " " + SingleBreakout.type);
                RemoveDrawObject("low_" + SingleBreakout.time + " " + SingleBreakout.type);
                SingleBreakout.b_high = false;
                SingleBreakout.b_low = false;
                SingleBreakout.high = 0;
                SingleBreakout.low = 0;
                SingleBreakout.barIndex = 0;
            }
            
        }
        private void FindEngulf()
        {
            double focusedBar_h = Highs[index][0];
            double focusedBar_l = Lows[index][0];
            double focusedBar_c = Closes[index][0];
            double focusedBar_o = Opens[index][0];
            double insideBar_h = Highs[index][1];
            double insideBar_l = Lows[index][1];
            if (focusedBar_h > insideBar_h && focusedBar_l < insideBar_l)
            {
                double ema = EMA(Closes[index], 21)[0];
                if (!(focusedBar_h > ema && focusedBar_l < ema))
                {
                    ema = EMA(Closes[index], 14)[0];
                    if (!(focusedBar_h > ema && focusedBar_l < ema))
                    {

                        return;
                    }
                }


                SingleBreakout.high = focusedBar_h;
                SingleBreakout.low = focusedBar_l;
                SingleBreakout.barIndex = CurrentBars[index];

                SingleBreakout.b_high = false;
                SingleBreakout.b_low = false;
                SingleBreakout._type = "Engulf";
                Draw.Line(this, "high_" + SingleBreakout.time + " " + SingleBreakout.type, false, 1, focusedBar_h, len * -1, focusedBar_h, brush1, DashStyleHelper.Solid, width);
                Draw.Line(this, "low_" + SingleBreakout.time + " " + SingleBreakout.type, false, 1, focusedBar_l, len * -1, focusedBar_l, brush1, DashStyleHelper.Solid, width);
            }
        }
        private void FindMother()
        {
            double focusedBar_h = Highs[index][1];
            double focusedBar_l = Lows[index][1];
            double focusedBar_c = Closes[index][1];
            double focusedBar_o = Opens[index][1];
            double insideBar_h = Highs[index][0];
            double insideBar_l = Lows[index][0];
            bool upBar = focusedBar_c > focusedBar_o;
            bool valid = false;

            if (focusedBar_h > insideBar_h && focusedBar_l < insideBar_l)
            {
                double ema = EMA(Closes[index], 21)[1];
                if (!(focusedBar_h > ema && focusedBar_l < ema))
                {
                    ema = EMA(Closes[index], 14)[1];
                    if (!(focusedBar_h > ema && focusedBar_l < ema)) 
                    {
                        return;
                    }
                }

                SingleBreakout.high = focusedBar_h;
                SingleBreakout.low = focusedBar_l;
                SingleBreakout.barIndex = CurrentBars[index];
                SingleBreakout.b_high = false;
                SingleBreakout.b_low = false;
                SingleBreakout._type = "Mother";
                Draw.Line(this, "high_" + SingleBreakout.time + " " + SingleBreakout.type, false, 1, focusedBar_h, len * -1, focusedBar_h, brush1, DashStyleHelper.Solid, width);
                Draw.Line(this, "low_" + SingleBreakout.time + " " + SingleBreakout.type , false, 1, focusedBar_l, len * -1, focusedBar_l, brush1, DashStyleHelper.Solid, width);
            }
        }


        #region Properties
        [NinjaScriptProperty]
        [Display(Name = "Enable Timeframe 1", Description = "", Order = 1, GroupName = "1) General")]
        public bool enable_timeframe1
        { get; set; }
        
        [NinjaScriptProperty]
        [Display(Name = "Time Frame 1 Type", Order = 2, GroupName = "1) General")]
        public Data.BarsPeriodType TF1Type { get; set; }
       
        [NinjaScriptProperty]
        [Display(Name = "Timeframe 1 Value", Description = "", Order = 3, GroupName = "1) General")]
        public int timeframe1_value
        { get; set; }
     
        [NinjaScriptProperty]
        [Display(Name = "Color 1", Order = 4, GroupName = "1) General")]
        public Brush color1
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Line Length", Description = "", Order = 5, GroupName = "2) Line Properties")]
        public int len
        { get; set; }
        [NinjaScriptProperty]
        [Display(Name = "Line Width", Description = "", Order = 6, GroupName = "2) Line Properties")]
        public int width
        { get; set; }

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private BreakoutSingleInterval[] cacheBreakoutSingleInterval;
		public BreakoutSingleInterval BreakoutSingleInterval(bool enable_timeframe1, Data.BarsPeriodType tF1Type, int timeframe1_value, Brush color1, int len, int width)
		{
			return BreakoutSingleInterval(Input, enable_timeframe1, tF1Type, timeframe1_value, color1, len, width);
		}

		public BreakoutSingleInterval BreakoutSingleInterval(ISeries<double> input, bool enable_timeframe1, Data.BarsPeriodType tF1Type, int timeframe1_value, Brush color1, int len, int width)
		{
			if (cacheBreakoutSingleInterval != null)
				for (int idx = 0; idx < cacheBreakoutSingleInterval.Length; idx++)
					if (cacheBreakoutSingleInterval[idx] != null && cacheBreakoutSingleInterval[idx].enable_timeframe1 == enable_timeframe1 && cacheBreakoutSingleInterval[idx].TF1Type == tF1Type && cacheBreakoutSingleInterval[idx].timeframe1_value == timeframe1_value && cacheBreakoutSingleInterval[idx].color1 == color1 && cacheBreakoutSingleInterval[idx].len == len && cacheBreakoutSingleInterval[idx].width == width && cacheBreakoutSingleInterval[idx].EqualsInput(input))
						return cacheBreakoutSingleInterval[idx];
			return CacheIndicator<BreakoutSingleInterval>(new BreakoutSingleInterval(){ enable_timeframe1 = enable_timeframe1, TF1Type = tF1Type, timeframe1_value = timeframe1_value, color1 = color1, len = len, width = width }, input, ref cacheBreakoutSingleInterval);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.BreakoutSingleInterval BreakoutSingleInterval(bool enable_timeframe1, Data.BarsPeriodType tF1Type, int timeframe1_value, Brush color1, int len, int width)
		{
			return indicator.BreakoutSingleInterval(Input, enable_timeframe1, tF1Type, timeframe1_value, color1, len, width);
		}

		public Indicators.BreakoutSingleInterval BreakoutSingleInterval(ISeries<double> input , bool enable_timeframe1, Data.BarsPeriodType tF1Type, int timeframe1_value, Brush color1, int len, int width)
		{
			return indicator.BreakoutSingleInterval(input, enable_timeframe1, tF1Type, timeframe1_value, color1, len, width);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.BreakoutSingleInterval BreakoutSingleInterval(bool enable_timeframe1, Data.BarsPeriodType tF1Type, int timeframe1_value, Brush color1, int len, int width)
		{
			return indicator.BreakoutSingleInterval(Input, enable_timeframe1, tF1Type, timeframe1_value, color1, len, width);
		}

		public Indicators.BreakoutSingleInterval BreakoutSingleInterval(ISeries<double> input , bool enable_timeframe1, Data.BarsPeriodType tF1Type, int timeframe1_value, Brush color1, int len, int width)
		{
			return indicator.BreakoutSingleInterval(input, enable_timeframe1, tF1Type, timeframe1_value, color1, len, width);
		}
	}
}

#endregion
