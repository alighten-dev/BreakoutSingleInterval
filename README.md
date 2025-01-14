All credit goes to flexusFutures for his great indicator Breakouts Multi-Interval. 

https://forum.ninjatrader.com/member/145226-flexusfuture
https://ninjatraderecosystem.com/user-app-share-download/breakouts-multi-interval/

I needed a simplified version that adds the higher timeframe DataSeries in a way that can be easily used in NinjaTrader Strategies. 

In this example I have two instances of the indicator added to the chart. One, configured for 60 minutes (Crimson), and a second configured for 120 minutes (Purple). The original indicator can handle eight timeframes with a single indicator. However, the method used to add the DataSeries made it difficult to use in NT strategies.

![image](https://github.com/user-attachments/assets/8c336d1d-8813-491d-aa64-6e8e173af809)

The original indicator and this simplified one work by finding candles on the higher timeframes that engulf their 21 and 14 EMAs. This technique works well for identifying important levels.

