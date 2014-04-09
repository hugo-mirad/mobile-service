/**********************************************************************
" MODULE       : Master
" FILENAME     : Master.cs
" AUTHOR       : Shravan
" CREATED      : 04-Jan-2012
" DESCRIPTION  : Business Logic to manipulate .
"*********************************************************************/

#region System References
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Services;
using CarsInfo;
//using CarsBL;


#endregion System References
//using CarsInfo;

#region Application References
using CarsInfo;
using CarsBL;
using CarsBL.Transactions;
using CarsBL.Masters;
using System.Web.Script.Services;
#endregion Application References

#region Microsoft Application Block References
//using Microsoft.Practices.EnterpriseLibrary.Data;
//using Microsoft.Practices.EnterpriseLibrary.Common;
//using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
//using Microsoft.Practices.EnterpriseLibrary.Logging;

#endregion Microsoft Application Block References

/// <summary>
/// Summary description for Class1
/// </summary>
public class Filter
{
    //
    // TODO: Add constructor logic here
    //
    public List<UsedCarsInfo> FilterData(List<UsedCarsInfo> objUsedCarsInfo, string FilterCondition)
    {
        List<UsedCarsInfo> objUsedCarsInfoFiltered = new List<UsedCarsInfo>();


        switch (FilterCondition)
        {
            case "Mileage1":


                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Mileage) <= Convert.ToDecimal(5000));

                break;

            case "Mileage2":

                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Mileage) >= 5000 && Convert.ToDecimal(x.Mileage) <= 10000);


                break;

            case "Mileage3":

                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Mileage) >= 10000 && Convert.ToDecimal(x.Mileage) <= 25000);

                break;


            case "Mileage4":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Mileage) >= 25000 && Convert.ToDecimal(x.Mileage) <= 50000);

                break;

            case "Mileage5":

                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Mileage) >= 50000 && Convert.ToDecimal(x.Mileage) <= 75000);


                break;

            case "Mileage6":

                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Mileage) >= 75000 && Convert.ToDecimal(x.Mileage) <= 100000);

                break;

            case "Mileage7":

                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Mileage) >= 100000);

                break;

            case "Year1":

                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.YearOfMake) == 2011);

                break;

            case "Year2":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.YearOfMake) == 2010);


                break;

            case "Year3":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.YearOfMake) == 2009);

                break;


            case "Year4":

                break;

            case "Year5":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.YearOfMake) == 2007);

                break;

            case "Year6":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.YearOfMake) == 2006);

                break;

            case "Year7":

                break;

            case "Price1":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Price) < 20000);

                break;

            case "Price2":

                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Price) < 20000 && Convert.ToDecimal(x.Price) < 50000);


                break;

            case "Price3":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Price) < 50000 && Convert.ToDecimal(x.Price) <= 75000);

                break;


            case "Price4":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Price) < 75000 && Convert.ToDecimal(x.Price) < 100000);

                break;

            case "Price5":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => Convert.ToDecimal(x.Price) >= 100000);
                break;

            case "Body1":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Convertible");

                break;

            case "Body2":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Coupe");

                break;

            case "Body3":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Hatchback");
                break;


            case "Body4":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Sedan");

                break;

            case "Body5":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "SUV");

                break;

            case "Body6":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Truck");

                break;

            case "Body7":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Van/Minivan");

                break;

            case "Body8":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Wagon");

                break;


            case "Body9":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Other");

                break;

            case "Fuel1":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Diesel");

                break;

            case "Fuel2":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Petrol");

                break;

            case "Fuel3":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Hybrid");

                break;


            case "Fuel4":

                break;

            case "Fuel5":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.Bodytype) == "Other");

                break;

            case "Seller1":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.SellerType) == "Invidual");

                break;

            case "Seller2":
                objUsedCarsInfoFiltered = objUsedCarsInfo.FindAll(x => (x.SellerType) == "Dealers");

                break;
        };

        return objUsedCarsInfoFiltered;
    }

}
