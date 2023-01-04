using System;
using System.Collections.Generic;
using System.Linq;

//The focus should be on clean, simple and easy to read code 
//Everything but the public interface may be changed
namespace TaxCalculatorInterviewTests
{
    /// <summary>
    /// This is the public inteface used by our client and may not be changed
    /// </summary>
    public interface ITaxCalculator
    {
        double GetStandardTaxRate(Commodity commodity);
        void SetCustomTaxRate(Commodity commodity, double rate);
        double GetTaxRateForDateTime(Commodity commodity, DateTime date);
        double GetCurrentTaxRate(Commodity commodity);
    }

    /// <summary>
    /// Implements a tax calculator for our client.
    /// The calculator has a set of standard tax rates that are hard-coded in the class.
    /// It also allows our client to remotely set new, custom tax rates.
    /// Finally, it allows the fetching of tax rate information for a specific commodity and point in time.
    /// TODO: We know there are a few bugs in the code below, since the calculations look messed up every now and then.
    ///       There are also a number of things that have to be implemented.
    /// </summary>
    public class TaxCalculator : ITaxCalculator
    {
        /// <summary>
        /// Get the standard tax rate for a specific commodity.
        /// </summary>
        public double GetStandardTaxRate(Commodity commodity)
        {
            if (commodity == Commodity.Default)
                return 0.25;
            if (commodity == Commodity.Alcohol)
                return 0.25;
            if (commodity == Commodity.Food)
                return 0.12;
            if (commodity == Commodity.FoodServices)
                return 0.12;
            if (commodity == Commodity.Literature)
                return 0.6;
            if (commodity == Commodity.Transport)
                return 0.6;
            if (commodity == Commodity.CulturalServices)
                return 0.6;

            return 0.25;
        }


        /// <summary>
        /// This method allows the client to remotely set new custom tax rates.
        /// When they do, we save the commodity/rate information as well as the UTC timestamp of when it was done.
        /// NOTE: Each instance of this object supports a different set of custom rates, since we run one thread per customer.
        /// </summary>
        public void SetCustomTaxRate(Commodity commodity, double rate)
        {
            //TODO: support saving multiple custom rates for different combinations of Commodity/DateTime
            //TODO: make sure we never save duplicates, in case of e.g. clock resets, DST etc - overwrite old values if this happens
            DateTime currentTime = DateTime.Now;
            //_customRates[commodity] = Tuple.Create(currentTime, rate);
            _comodityCustomRates.TryGetValue(commodity, out List<RateWithDateTime> rates);

            if (rates != null)
            {
                if (!rates.Any(a => a._dateTime == currentTime))
                    rates.Add(new RateWithDateTime { _dateTime = currentTime, _rate = rate });
                else
                    rates.Where(a => a._dateTime == currentTime).First()._rate = rate;
            }
            else
            {
                RateWithDateTime newRate = new RateWithDateTime { _dateTime = currentTime, _rate = rate };
                _comodityCustomRates.Add(commodity, new List<RateWithDateTime> { newRate });
            }
        }


        Dictionary<Commodity, List<RateWithDateTime>> _comodityCustomRates = new Dictionary<Commodity, List<RateWithDateTime>>();


        /// <summary>
        /// Gets the tax rate that is active for a specific point in time (in UTC).
        /// A custom tax rate is seen as the currently active rate for a period from its starting timestamp until a new custom rate is set.
        /// If there is no custom tax rate for the specified date, use the standard tax rate.
        /// </summary>
        public double GetTaxRateForDateTime(Commodity commodity, DateTime date)
        {
            bool commodityExists = _comodityCustomRates.TryGetValue(commodity, out List<RateWithDateTime> rates);
            if (commodityExists)
            {
                var rateWithDate = rates.Where(a => a._dateTime == date).ToList();
                if (rateWithDate.Any())
                    return rateWithDate.First()._rate;
            }
            return GetStandardTaxRate(commodity);
        }

        /// <summary>
        /// Gets the tax rate that is active for the current point in time.
        /// A custom tax rate is seen as the currently active rate for a period from its starting timestamp until a new custom rate is set.
        /// If there is no custom tax currently active, use the standard tax rate.
        /// </summary>
        public double GetCurrentTaxRate(Commodity commodity)
        {
            bool commodityExists = _comodityCustomRates.TryGetValue(commodity, out List<RateWithDateTime> rates);
            if (commodityExists)
            {
                var rateWithDate = rates.OrderByDescending(a => a._dateTime).ToList();
                if (rateWithDate.Any())
                    return rateWithDate.First()._rate;
            }

           

            return GetStandardTaxRate(commodity);
        }

    }

    public enum Commodity
    {
        //PLEASE NOTE: THESE ARE THE ACTUAL TAX RATES THAT SHOULD APPLY, WE JUST GOT THEM FROM THE CLIENT!
        Default,            //25%
        Alcohol,            //25%
        Food,               //12%
        FoodServices,       //12%
        Literature,         //6%
        Transport,          //6%
        CulturalServices    //6%
    }

    


    public class RateWithDateTime 
    {
        public DateTime _dateTime { set; get; }
        public double _rate { set; get; }
    
    }
}


