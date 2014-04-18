using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CarsBL.Masters;
using CarsInfo;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Web;
using System.ServiceModel.Web;
using CarsBL.Transactions;
using CarsBL;
using System.Collections;
using System.Data;
using CarsBL.Dealer;
using System.Configuration;
using System.Net.Mail;
using System.ServiceModel.Channels;


public class ServiceMobile : IServiceMobile
{

    public List<CarsInfo.UserLoginInfo> PerformLoginMobile(string Username, string Password, string AuthenticationID, string CustomerID)
    {
        MobileBL objUser = new MobileBL();
        var obj = new List<CarsInfo.UserLoginInfo>();
        try
        {
            if (CustomerID.Trim() != "")
            {
                DataSet dsSaveCustInfo = objUser.SaveMobileCustomerInfo("PerformLoginMobile", CustomerID, AuthenticationID, Username);
            }

            if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
            {
                obj = (List<CarsInfo.UserLoginInfo>)objUser.PerformLoginMobile(Username, Password);
                if (obj.Count > 0)
                {
                                
                    int UserID = Convert.ToInt32(obj[0].UID.ToString());
                    DataSet ds = objUser.MobileSaveUserLog(UserID);
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            obj[0].SessionID = ds.Tables[0].Rows[0]["M_UCESessionID"].ToString();
                        }
                    }
                }
            }
            if (obj.Count <= 0)
            {
                CarsInfo.UserLoginInfo objinfo = new UserLoginInfo();
                objinfo.AASuccess = "Failure";
                obj.Add(objinfo);
            }
        }
        catch (Exception ex)
        {
        }

        return obj;


    }

    public string PerformLogoutMobile(string UserID,string SessionID,string AuthenticationID, string CustomerID)
    {
        MobileBL objUser = new MobileBL();
        string bStatus = "Failure";
        try
        {
            if (CustomerID.Trim() != "")
            {
                DataSet dsSaveCustInfo = objUser.SaveMobileCustomerInfo("PerformLogoutMobile", CustomerID, AuthenticationID,UserID);
            }

            if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
            {
               bool bnew=objUser.PerformLogoutMobile(SessionID, Convert.ToInt32(UserID));
               if (bnew)
               {
                    bStatus = "Success";
               }
            }
        }
        catch (Exception ex)
        {
        }

        return bStatus;
    }

    public List<CarsInfo.MobileUserRegData> GetUserRegistrationDetailsByID(string UID, string AuthenticationID, string CustomerID,string SessionID)
    {

        List<CarsInfo.MobileUserRegData> obj = new List<CarsInfo.MobileUserRegData>();

        MobileBL objReg = new MobileBL();
        try
        {
            if (CustomerID.Trim() != "")
            {
                DataSet dsSaveCustInfo = objReg.SaveMobileCustomerInfo("GetUserRegistrationDetailsByID", CustomerID,AuthenticationID,UID);
            }
            bool bnew = objReg.CheckMobileAuthorizeUSer(SessionID,Convert.ToInt32(UID));
            if (bnew)
            {
                if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
                {
                    obj = (List<CarsInfo.MobileUserRegData>)objReg.GetUSerDetailsByUserID(Convert.ToInt32(UID));

                }
            }
            else
            {
                var obj1= new CarsInfo.MobileUserRegData();
                obj1.AASucess = "Session timed out";
                obj.Add(obj1);

            }
        }
        catch (Exception ex)
        {
        }
        return obj;
    }

    public List<CarsInfo.SalesInfo> SalesAgentLogin(string Username, string Password, string CenterCode)
    {
        List<SalesInfo> objSalesList = new List<SalesInfo>();
        CarsBL.Transactions.MobileBL objReg = new CarsBL.Transactions.MobileBL();
        DataSet dsGetCenterInfo = objReg.GetCenterData(CenterCode);
        if (dsGetCenterInfo.Tables.Count > 0)
        {
            if (dsGetCenterInfo.Tables[0].Rows.Count > 0)
            {
                if (dsGetCenterInfo.Tables[0].Rows[0]["AgentCenterStatus"].ToString() == "1")
                {
                    DataSet dsUserDetails = new DataSet();
                    dsUserDetails = objReg.HotLeadsPerformLogin(Username, Password, CenterCode);
                    if (dsUserDetails.Tables.Count > 0)
                    {
                        if (dsUserDetails.Tables[0].Rows.Count > 0)
                        {
                            string AgentCenterCode = dsUserDetails.Tables[0].Rows[0]["AgentCenterCode"].ToString();
                            string CenterID = dsUserDetails.Tables[0].Rows[0]["AgentCenterID"].ToString();
                            string AgentName = dsUserDetails.Tables[0].Rows[0]["AgentUFirstName"].ToString();
                            DataSet dsDatetime = objReg.GetDatetime();
                            DateTime dtNow = Convert.ToDateTime(dsDatetime.Tables[0].Rows[0]["Datetime"].ToString());
                            string Date = dtNow.ToString("MM/dd/yyyy hh:mm tt");
                            DataSet dsData = objReg.GetAllSalesByCenterForTicker(Convert.ToInt32(CenterID));
                            DataSet dsAllCenters = objReg.GetAllCenterSalesByCenterForTicker(Convert.ToInt32(CenterID));
                            int TotalSales = Convert.ToInt32(dsData.Tables[0].Compute("sum(Count)", ""));
                            Double TotalAmount = Convert.ToDouble(dsData.Tables[0].Compute("sum(TotalAmount)", ""));
                            string Totalsales = TotalSales.ToString() + " ($" + string.Format("{0:0.00}", TotalAmount).ToString() + ")";

                            if (dsData.Tables.Count > 0)
                            {
                                if (dsData.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                                    {
                                        SalesInfo objsalesInfo = new SalesInfo();
                                        objsalesInfo.Date = Date.ToString();
                                        objsalesInfo.MainCenter = AgentCenterCode.ToString();
                                        objsalesInfo.SalesAgentName = dsData.Tables[0].Rows[i]["SaleAgent"].ToString();
                                        objsalesInfo.AgentSales = dsData.Tables[0].Rows[i]["Count"].ToString();
                                        Double AgentSalesAmount = Convert.ToDouble(dsData.Tables[0].Rows[i]["TotalAmount"].ToString());
                                        objsalesInfo.AgentSalesAmount = string.Format("{0:0.00}", AgentSalesAmount).ToString();
                                        objsalesInfo.CenterCode = AgentCenterCode.ToString();
                                        objsalesInfo.CenterSalesAmount = string.Format("{0:0.00}", TotalAmount).ToString();
                                        objsalesInfo.CenterSalesCount = TotalSales.ToString();
                                        objSalesList.Add(objsalesInfo);
                                    }
                                }
                            }
                            if (dsAllCenters.Tables.Count > 0)
                            {
                                if (dsAllCenters.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < dsAllCenters.Tables[0].Rows.Count; i++)
                                    {
                                        SalesInfo objsalesInfo = new SalesInfo();
                                        objsalesInfo.Date = Date.ToString();
                                        objsalesInfo.MainCenter = AgentCenterCode.ToString();
                                        objsalesInfo.CenterCode = dsAllCenters.Tables[0].Rows[i]["Center"].ToString();
                                        Double CenterSalesAmount = Convert.ToDouble(dsAllCenters.Tables[0].Rows[i]["TotalAmount"].ToString());
                                        objsalesInfo.CenterSalesAmount = string.Format("{0:0.00}", CenterSalesAmount).ToString();
                                        objsalesInfo.CenterSalesCount = dsAllCenters.Tables[0].Rows[i]["Count"].ToString();
                                        objSalesList.Add(objsalesInfo);
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }
        return objSalesList;
    }

    public List<UsedCarsInfo> GetRecentCarsMobile(string sCurrentPage, string PageSize,
           string Orderby, string Sort, string sPin, string AuthenticationID, string CustomerID)
    {

        CarsBL.Transactions.MobileData objCarsearch = new CarsBL.Transactions.MobileData();
        MobileBL objMobileBL = new MobileBL();
        var obj = new List<CarsInfo.UsedCarsInfo>();

        try
        {
            if (CustomerID.Trim() != "")
            {
             string parameters=Orderby+","+Sort+","+sPin;
                DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("GetRecentCarsMobile", CustomerID,AuthenticationID,parameters);
            }

            if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
            {
                obj = (List<CarsInfo.UsedCarsInfo>)objCarsearch.GetRecentCarsMobile(sCurrentPage, PageSize, Orderby, Sort, sPin);
               
            }

        }
        catch (Exception ex)
        {
        }
        return obj;



    }

    public bool SaveCallRequestMobile(string BuyerPhoneNo, string CarID, string CustomerPhoneNo, string AuthenticationID, string CustomerID)
    {
        MobileBL objMobileBL = new MobileBL();
        CallRequestMobileBL objCallRequestMobile = new CallRequestMobileBL();
        if (CustomerID.Trim() != "")
        {
            string parameters=BuyerPhoneNo+","+CarID+","+CustomerPhoneNo;
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("SaveCallRequestMobile", CustomerID,AuthenticationID,parameters);
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            objCallRequestMobile.SaveCallRequestMobile(BuyerPhoneNo, CarID, CustomerPhoneNo);
          
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SaveBuyerRequestMobile(string BuyerEmail, string BuyerCity,
           string BuyerPhone, string BuyerFirstName, string BuyerLastName, string BuyerComments,
           string IpAddress, string Sellerphone, string Sellerprice, string Carid,
           string sYear, string Make, string Model, string price, string ToEmail, string AuthenticationID, string CustomerID)
    {
        BuyerTranBL objBuyerTranBL = new BuyerTranBL();
        MobileBL objMobileBL = new MobileBL();

        if (CustomerID.Trim() != "")
        {
              string parameters=BuyerPhone+","+Sellerphone+","+Carid;
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("SaveBuyerRequestMobile", CustomerID,AuthenticationID,parameters);
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            objBuyerTranBL.SaveBuyerRequestMobile(BuyerEmail, BuyerCity,
               BuyerPhone, BuyerFirstName, BuyerLastName, BuyerComments,
               IpAddress, Sellerphone, Sellerprice, Carid,
                sYear, Make, Model, price, "1", ToEmail);

            
            return true;
        }
        else
        {
            return false;
        }
    }

    public ArrayList GetCarFeatures(string sCarId, string AuthenticationID, string CustomerID)
    {
        DataSet ds = new DataSet();
        CarFeatures objCarFeatures = new CarFeatures();
        ArrayList arr = new ArrayList();
        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("GetCarFeatures", CustomerID,AuthenticationID,sCarId);
        }


        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            ds = objCarFeatures.GetCarFeatures(sCarId);

            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    arr.Add(ds.Tables[0].Rows[i]["FeatureTypeName"].ToString() + "," + ds.Tables[0].Rows[i]["FeatureName"].ToString());
                }
            }
            
        }
        return arr;
    }

    public List<CarsInfo.UsedCarsInfo> GetCarsSearchJSON(string carMakeid, string CarModalId, string ZipCode, string WithinZip, string pageNo, string pageresultscount, string orderby, string AuthenticationID, string CustomerID)
    {

        CarsBL.UsedCarsSearch objCarsearch = new CarsBL.UsedCarsSearch();

        var obj = new List<CarsInfo.UsedCarsInfo>();

        MobileBL objMobileBL = new MobileBL();
        string sort = string.Empty;
        if (orderby != "")
        {

            orderby = "price";
        }
        if (sort != "")
        {
            sort = "desc";
        }

        string IPAddress = string.Empty;

        string SearchName = string.Empty;

        if (CustomerID.Trim() != "")
        {
            string parameters=carMakeid+","+CarModalId+","+ZipCode+","+WithinZip+","+orderby;
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("GetCarsSearchJSON", CustomerID,AuthenticationID,parameters);
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            obj = (List<CarsInfo.UsedCarsInfo>)objCarsearch.SearchUsedCars(carMakeid, CarModalId, ZipCode, WithinZip, pageNo, pageresultscount, orderby, sort);
          
        }
        return obj;
    }

    public List<MakesInfo> GetMakes(string AuthenticationID, string CustomerID)
    {

        var obj = new List<MakesInfo>();


        MakesBL objMakesBL = new MakesBL();

        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("GetMakes", CustomerID,AuthenticationID,"AllMakes");
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            obj = (List<MakesInfo>)objMakesBL.GetMakes();
            
        }
        return obj;

    }

    public List<ModelsInfo> GetModelsInfo(string AuthenticationID, string CustomerID)
    {
        ModelBL objModelBL = new ModelBL();

        var obj = new List<ModelsInfo>();
        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("GetModelsInfo", CustomerID,AuthenticationID,"AllModels");
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            obj = (List<ModelsInfo>)objModelBL.GetModels("0");
          
        }
        return obj;

    }


    public List<CarsInfo.UsedCarsInfo> GetCarsFilterMobile(string carMakeID, string CarModalId,
                                       string Mileage, string Year, string Price, string Sort, string Orderby, string pageSize, string CurrentPage, string Zipcode, string AuthenticationID, string CustomerID)
    {

        CarsFilter objCarsFilter = new CarsFilter();

      
        Filter objFilter = new Filter();

        List<CarsInfo.UsedCarsInfo> objFilterdata = new List<CarsInfo.UsedCarsInfo>();


        CarsInfo.UsedCarsInfo OBJ = new CarsInfo.UsedCarsInfo();



        string sort = string.Empty;

        objCarsFilter.CurrentPage = CurrentPage;
        objCarsFilter.PageSize = pageSize;
        objCarsFilter.CarMakeid = carMakeID;
        objCarsFilter.CarModalId = CarModalId;
        objCarsFilter.Sort = Sort;
        objCarsFilter.Orderby = Orderby;
        objCarsFilter.ZipCode = Zipcode;



        objCarsFilter.Sort = sort;



        switch (Mileage)
        {
            case "Mileage1":
                objCarsFilter.Mileage1 = "Mileage1";
                break;
            case "Mileage2":
                objCarsFilter.Mileage2 = "Mileage2";
                break;
            case "Mileage3":
                objCarsFilter.Mileage3 = "Mileage3";
                break;
            case "Mileage4":
                objCarsFilter.Mileage4 = "Mileage4";
                break;
            case "Mileage5":
                objCarsFilter.Mileage5 = "Mileage5";
                break;
            case "Mileage6":
                objCarsFilter.Mileage6 = "Mileage6";
                break;
            case "Mileage7":
                objCarsFilter.Mileage7 = "Mileage7";
                break;
        }
        switch (Year)
        {
            case "Year1a":
                objCarsFilter.Year1a = "Year1a";
                break;
            case "Year1b":
                objCarsFilter.Year1b = "Year1b";
                break;
            case "Year1":
                objCarsFilter.Year1 = "Year1";
                break;
            case "Year2":
                objCarsFilter.Year2 = "Year2";
                break;
            case "Year3":
                objCarsFilter.Year3 = "Year3";
                break;
            case "Year4":
                objCarsFilter.Year4 = "Year4";
                break;
            case "Year5":
                objCarsFilter.Year5 = "Year5";
                break;
            case "Year6":
                objCarsFilter.Year6 = "Year6";
                break;
            case "Year7":
                objCarsFilter.Year7 = "Year7";
                break;
        }
        switch (Price)
        {
            case "Price1":
                objCarsFilter.Price1 = "Price1";
                break;
            case "Price2":
                objCarsFilter.Price2 = "Price2";
                break;
            case "Price3":
                objCarsFilter.Price3 = "Price3";
                break;
            case "Price4":
                objCarsFilter.Price4 = "Price4";
                break;
            case "Price5":
                objCarsFilter.Price5 = "Price5";
                break;
        };


        FilterCars objFilterCars = new FilterCars();
        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
              string parameter=carMakeID+","+CarModalId+","+Mileage+","+Year+","+Price+","+Sort+","+Orderby+","+Zipcode;
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("GetCarsFilterMobile", CustomerID,AuthenticationID,parameter);
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            objFilterdata = (List<CarsInfo.UsedCarsInfo>)objFilterCars.FilterSearchMobile(objCarsFilter);
            
        }
        return objFilterdata;

    }


    public List<CarsInfo.UsedCarsInfo> FindCarID(string sCarid, string AuthenticationID, string CustomerID)
    {

        List<CarsInfo.UsedCarsInfo> obUsedCarsInfo = new List<CarsInfo.UsedCarsInfo>();
        UsedCarsSearch obj = new UsedCarsSearch();

        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("FindCarID", CustomerID,AuthenticationID,sCarid);
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            obUsedCarsInfo = (List<CarsInfo.UsedCarsInfo>)obj.FindCarID(sCarid);
           
        }
        return obUsedCarsInfo;
    }


    public List<CarsInfo.PackagesInfo> GetPackageDetailsByUID(string UID, string AuthenticationID, string CustomerID,string SessionID)
    {
        List<CarsInfo.PackagesInfo> objPackagesInfo = new List<CarsInfo.PackagesInfo>();

        MobileBL objMobileBL = new MobileBL();
        try
        {
            if (CustomerID.Trim() != "")
            {
                DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("GetPackageDetailsByUID", CustomerID, AuthenticationID,UID);
            }
            bool bnew = objMobileBL.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
            if (bnew)
            {
                if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
                {
                    objPackagesInfo = (List<CarsInfo.PackagesInfo>)objMobileBL.GetPackageDetailsBYUID(UID);

                }
            }
            else
            {
                PackagesInfo objPack = new PackagesInfo();
                objPack.AASuccess = "Session timed out";
                objPackagesInfo.Add(objPack);

            }
        }
        catch (Exception ex)
        {
        }

        return objPackagesInfo;
    }

    public bool SendMobileRegistrationRequest(string name, string phonenumber, string email, string AuthenticationID, string CustomerID)
    {
        MobileBL objMobileBL = new MobileBL();
        bool bSuccess = false;

        if (CustomerID.Trim() != "")
        {
            string parameter = name + "," + phonenumber + "," + email;
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("SendMobileRegistrationRequest", CustomerID,AuthenticationID,parameter);
        }

        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            NewCarsBl objNewReq = new NewCarsBl();
            try
            {
                string ReqName = name.Trim();
                string ReqPhone = phonenumber;
                string ReqEmail = email;
                DataSet dsNewCarRequest = new DataSet();
                dsNewCarRequest = objNewReq.SaveMobileRegistrationRequest(ReqName, ReqPhone, ReqEmail);

                if (dsNewCarRequest.Tables[0].Rows.Count > 0)
                {
                    string NewCarName = dsNewCarRequest.Tables[0].Rows[0]["MobileRegReqName"].ToString();
                    string Phone = dsNewCarRequest.Tables[0].Rows[0]["MobileRegReqPhone"].ToString();
                    string Email = dsNewCarRequest.Tables[0].Rows[0]["MobileRegReqEmail"].ToString();

                    if (Email == "")
                    {
                        Email = System.Configuration.ConfigurationSettings.AppSettings["From"].ToString();
                    }

                    MailFormats format = new MailFormats();
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress(Email);

                    msg.To.Add(System.Configuration.ConfigurationSettings.AppSettings["To"].ToString());
                    msg.CC.Add(System.Configuration.ConfigurationSettings.AppSettings["CC"].ToString());
                    msg.Subject = "Regarding  mobile registration request";
                    msg.IsBodyHtml = true;
                    string text = string.Empty;
                    text = format.SendNewRegistrationRequestDetails(NewCarName, Phone, Email, ref text);
                    msg.Body = text.ToString();
                    SmtpClient smtp = new SmtpClient();
                    //smtp.Host = "smtp.gmail.com";
                    //smtp.Port = 587;
                    //smtp.Credentials = new System.Net.NetworkCredential("shobha@datumglobal.net", "sob902290");
                    //smtp.EnableSsl = true;
                    //smtp.Send(msg);
                    smtp.Host = "127.0.0.1";
                    smtp.Port = 25;
                    smtp.Send(msg);

                    bSuccess = true;
                }
            }
            catch (Exception ex)
            {
            }

        }
        

        return bSuccess;
    }


    public List<UserRegistrationInfo> UpdateUserRegistration(string name,string address,string city,string stateID,string zip,string phone,string UID,
                                                             string businessName, string altEmail, string altPhone, string AuthenticationID, string CustomerID, string SessionID)
    {
        MobileBL objUserRegBL = new MobileBL();
        var obj =new List<UserRegistrationInfo>();
        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("UpdateUserRegistration", CustomerID, AuthenticationID,UID);
        }
         bool bnew = objMobileBL.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
         if (bnew)
         {
             try
             {
                 if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
                 {
                     UserRegistrationInfo objUserInfo = new UserRegistrationInfo();

                     objUserInfo.Name = GeneralFunc.ToProper(name).Trim();
                     objUserInfo.Address = GeneralFunc.ToProper(address).Trim();
                     objUserInfo.City = GeneralFunc.ToProper(city).Trim();
                     objUserInfo.StateID = Convert.ToInt32(stateID);
                     if (zip.Length == 4)
                     {
                         objUserInfo.Zip = "0" + zip;
                     }
                     else
                     {
                         objUserInfo.Zip = zip;
                     }
                     objUserInfo.PhoneNumber = phone;

                     objUserInfo.UId = Convert.ToInt32(UID);
                     objUserInfo.BusinessName = businessName;
                     objUserInfo.AltEmail = altEmail;
                     objUserInfo.AltPhone = altPhone;
                     DataSet dsCarDetailsInfo = new DataSet();
                     dsCarDetailsInfo = objUserRegBL.USP_UpdateRegUserDetails(objUserInfo);

                     if (dsCarDetailsInfo.Tables.Count > 0)
                     {
                         if (dsCarDetailsInfo.Tables[0].Rows.Count > 0)
                         {
                             UserRegistrationInfo objInfo = new UserRegistrationInfo();
                             objInfo.AASuccess = "Success";
                             objInfo.Address = dsCarDetailsInfo.Tables[0].Rows[0]["Address"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["Address"].ToString();
                             objInfo.AltEmail = dsCarDetailsInfo.Tables[0].Rows[0]["AltEmail"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["AltEmail"].ToString();
                             objInfo.AltPhone = dsCarDetailsInfo.Tables[0].Rows[0]["AltPhone"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["AltPhone"].ToString();
                             objInfo.BusinessName = dsCarDetailsInfo.Tables[0].Rows[0]["BusinessName"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["BusinessName"].ToString();
                             objInfo.City = dsCarDetailsInfo.Tables[0].Rows[0]["City"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["City"].ToString();
                             objInfo.CouponCode = dsCarDetailsInfo.Tables[0].Rows[0]["CouponCode"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["CouponCode"].ToString();
                             objInfo.CreatedDate = dsCarDetailsInfo.Tables[0].Rows[0]["CreatedDate"].ToString() == "" ? Convert.ToDateTime("1/1/1990") : Convert.ToDateTime(dsCarDetailsInfo.Tables[0].Rows[0]["CreatedDate"].ToString());
                             objInfo.Name = dsCarDetailsInfo.Tables[0].Rows[0]["Name"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["Name"].ToString();
                             objInfo.PhoneNumber = dsCarDetailsInfo.Tables[0].Rows[0]["PhoneNumber"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["PhoneNumber"].ToString();
                             objInfo.SellerID = dsCarDetailsInfo.Tables[0].Rows[0]["sellerID"].ToString() == "" ? 0 : Convert.ToInt32(dsCarDetailsInfo.Tables[0].Rows[0]["sellerID"].ToString());
                             objInfo.StateID = dsCarDetailsInfo.Tables[0].Rows[0]["StateID"].ToString() == "" ? 0 : Convert.ToInt32(dsCarDetailsInfo.Tables[0].Rows[0]["StateID"].ToString());
                             objInfo.StateName = dsCarDetailsInfo.Tables[0].Rows[0]["State_Code"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["State_Code"].ToString();
                             objInfo.UId = dsCarDetailsInfo.Tables[0].Rows[0]["UId"].ToString() == "" ? 0 : Convert.ToInt32(dsCarDetailsInfo.Tables[0].Rows[0]["UId"].ToString());
                             objInfo.UserName = dsCarDetailsInfo.Tables[0].Rows[0]["UserName"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["UserName"].ToString();
                             objInfo.Zip = dsCarDetailsInfo.Tables[0].Rows[0]["Zip"].ToString() == "" ? "Emp" : dsCarDetailsInfo.Tables[0].Rows[0]["Zip"].ToString();

                             obj.Add(objInfo);

                         }
                     }

                 }
             }
             catch (Exception ex)
             {
             }
         }
         else
         {
             UserRegistrationInfo objUserInfo = new UserRegistrationInfo();
             objUserInfo.AASuccess = "Session timed out";
             obj.Add(objUserInfo);
         }
        return obj;
    }


    public string UpdateSellerInformation(string sellerID, string sellerName,string city, string state, string zip, string phone, string email, string carID, string UID, string AuthenticationID, string CustomerID, string SessionID)
    {
        string returnPostingID = "Failure";
        
        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("UpdateSellerInformation", CustomerID, AuthenticationID,sellerID);
        }
         bool bnew = objMobileBL.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
         if (bnew)
         {
             try
             {
                 if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
                 {
                     UsedCarsInfo objUsedCarsInfo = new UsedCarsInfo();
                     objUsedCarsInfo.SellerID = Convert.ToInt32(sellerID);
                     objUsedCarsInfo.SellerName = sellerName;
                     objUsedCarsInfo.City = city;
                     objUsedCarsInfo.State = state;
                     objUsedCarsInfo.Zip = zip;
                     objUsedCarsInfo.Phone = phone;
                     objUsedCarsInfo.Email = email;

                     DataSet dsposting = new DataSet();
                     dsposting = objMobileBL.UpdateMobileSellerInfo(objUsedCarsInfo, Convert.ToInt32(carID), Convert.ToInt32(UID));

                     if (dsposting.Tables.Count > 0)
                     {
                         if (dsposting.Tables[0].Rows.Count > 0)
                         {
                             returnPostingID = "Success";
                         }
                     }
                 }
             }
             catch (Exception ex)
             {
             }
         }
         else
         {
             returnPostingID = "Session timed out";
         }
        return returnPostingID;
    }


    public string UpdateCarDescriptionByCarID(string Description, string CarID,string UID,string AuthenticationID, string CustomerID, string SessionID)
    {
        string returnPostingID = "Failure";
        MobileBL objMobileBL = new MobileBL();
       
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("UpdateCarDescriptionByCarID", CustomerID, AuthenticationID,CarID);
        }
         bool bnew = objMobileBL.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
         if (bnew)
         {
             try
             {
                 if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
                 {
                     bool bnw = objMobileBL.UpdateMobileDescriptionByCarID(Convert.ToInt32(CarID), Description);
                     if (bnw)
                     {
                         returnPostingID = "Success";
                     }
                 }

             }
             catch (Exception ex)
             {

             }
         }
         else
         {
             returnPostingID = "Session timed out";
         }
        return returnPostingID;
    }

    public string UpdateMobileCarStatusByCarID(string CarID, string UID,string AdstatusName,string AuthenticationID, string CustomerID, string SessionID)
    {

        string returnPostingID = "Failure";
        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("UpdateMobileCarStatusByCarID", CustomerID, AuthenticationID, CarID);
        }
         bool bnew = objMobileBL.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
         if (bnew)
         {

             try
             {
                 bool bnw = objMobileBL.UpdateMobileCarStatusByCarID(Convert.ToInt32(CarID), Convert.ToInt32(UID), AdstatusName);
                 if (bnw)
                 {
                     returnPostingID = "Success";
                 }
             }
             catch (Exception ex)
             {

             }
         }
         else
         {
             returnPostingID = "Session timed out";
         }
         return returnPostingID;
    }


    public List<CarsInfo.UsedCarsInfo> FindMobileCarsByUID(string UID, string AuthenticationID, string CustomerID)
    {
        List<CarsInfo.UsedCarsInfo> obUsedCarsInfo = new List<CarsInfo.UsedCarsInfo>();
        UsedCarsSearch obj = new UsedCarsSearch();

        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("FindMobileCarsByUID", CustomerID, AuthenticationID,UID);
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            obUsedCarsInfo = (List<CarsInfo.UsedCarsInfo>)objMobileBL.MopbileFindCarsByUID(UID);

        }

        return obUsedCarsInfo;

    }




    public List<CarsInfo.UsedCarsInfo> GetCarsFilterAndroidMobile(string carMake, string CarModal,
                                  string Mileage, string Year, string Price, string Sort, string Orderby, string pageSize, string CurrentPage, string Zipcode, string AuthenticationID, string CustomerID)
    {

        CarsFilter objCarsFilter = new CarsFilter();


        Filter objFilter = new Filter();

        List<CarsInfo.UsedCarsInfo> objFilterdata = new List<CarsInfo.UsedCarsInfo>();


        CarsInfo.UsedCarsInfo OBJ = new CarsInfo.UsedCarsInfo();



        string sort = string.Empty;

        objCarsFilter.CurrentPage = CurrentPage;
        objCarsFilter.PageSize = pageSize;
        objCarsFilter.CarMakeid = carMake;
        objCarsFilter.CarModalId = CarModal;
        objCarsFilter.Sort = Sort;
        objCarsFilter.Orderby = Orderby;
        objCarsFilter.ZipCode = Zipcode;



        objCarsFilter.Sort = sort;



        switch (Mileage)
        {
            case "Mileage1":
                objCarsFilter.Mileage1 = "Mileage1";
                break;
            case "Mileage2":
                objCarsFilter.Mileage2 = "Mileage2";
                break;
            case "Mileage3":
                objCarsFilter.Mileage3 = "Mileage3";
                break;
            case "Mileage4":
                objCarsFilter.Mileage4 = "Mileage4";
                break;
            case "Mileage5":
                objCarsFilter.Mileage5 = "Mileage5";
                break;
            case "Mileage6":
                objCarsFilter.Mileage6 = "Mileage6";
                break;
            case "Mileage7":
                objCarsFilter.Mileage7 = "Mileage7";
                break;
        }
        switch (Year)
        {
            case "Year1a":
                objCarsFilter.Year1a = "Year1a";
                break;
            case "Year1b":
                objCarsFilter.Year1b = "Year1b";
                break;
            case "Year1":
                objCarsFilter.Year1 = "Year1";
                break;
            case "Year2":
                objCarsFilter.Year2 = "Year2";
                break;
            case "Year3":
                objCarsFilter.Year3 = "Year3";
                break;
            case "Year4":
                objCarsFilter.Year4 = "Year4";
                break;
            case "Year5":
                objCarsFilter.Year5 = "Year5";
                break;
            case "Year6":
                objCarsFilter.Year6 = "Year6";
                break;
            case "Year7":
                objCarsFilter.Year7 = "Year7";
                break;
        }
        switch (Price)
        {
            case "Price1":
                objCarsFilter.Price1 = "Price1";
                break;
            case "Price2":
                objCarsFilter.Price2 = "Price2";
                break;
            case "Price3":
                objCarsFilter.Price3 = "Price3";
                break;
            case "Price4":
                objCarsFilter.Price4 = "Price4";
                break;
            case "Price5":
                objCarsFilter.Price5 = "Price5";
                break;
        };


        FilterCars objFilterCars = new FilterCars();
        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            string parameter = carMake + "," + CarModal + "," + Mileage + "," + Year + "," + Price + "," + Sort + "," + Orderby + "," + Zipcode;
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("GetCarsFilterMobile", CustomerID, AuthenticationID, parameter);
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            objFilterdata = (List<CarsInfo.UsedCarsInfo>)objFilterCars.FilterSearchMobileAndroid(objCarsFilter);

        }
        return objFilterdata;

    }




    public List<CarsInfo.MultisiteInfo> GetMultisiteListingsByCarID(string CarID, string AuthenticationID, string CustomerID, string SessionID,string UID)
    {
        List<CarsInfo.MultisiteInfo> obj = new List<CarsInfo.MultisiteInfo>();
        MobileBL objMobileBL = new MobileBL();
        MultisiteInfo objCarInfo = new MultisiteInfo();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("GetMultisiteListingsByCarID", CustomerID, AuthenticationID,CarID);
        }
        try
        {

            bool bnew = objMobileBL.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
            if (bnew)
            {
                int carIDnew = CarID == "" ? 0 : Convert.ToInt32(CarID);

                obj = (List<CarsInfo.MultisiteInfo>)objMobileBL.GetMultiSitePostingsByCariD(carIDnew);

                if (obj.Count <=0)
                {

                    objCarInfo.AASuccess = "Failure";
                    obj.Add(objCarInfo);
                }

            }
            else
            {
                objCarInfo.AASuccess = "Session timed out";
                obj.Add(objCarInfo);
            }

            
        }
        catch (Exception ex)
        {
        }
        return obj;


    }

    public List<MakeCountInfo> GetMakeCounts(string AuthenticationID, string CustomerID)
    {

        var obj = new List<MakeCountInfo>();


        MakesBL objMakesBL = new MakesBL();

        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("GetMakes", CustomerID, AuthenticationID,"AllMakes");
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            obj = (List<MakeCountInfo>)objMobileBL.GetMakeCounts();
            

        }
        return obj;

    }

    public List<CarsInfo.UsedCarsInfo> MobileCarsNotificationSearch(string carMake, string CarModal,
                                 string Mileage, string Year, string Price, string Orderby, string Zipcode, string exteriorColor, string interiorColor,
           string Transmission, string VehiceCondition, string DriveTrain, string numberOfCylinder, string numberOfDoors, string fuelType, string AuthenticationID, string CustomerID)
    {

        CarsFilter objCarsFilter = new CarsFilter();


        Filter objFilter = new Filter();

        List<CarsInfo.UsedCarsInfo> objFilterdata = new List<CarsInfo.UsedCarsInfo>();


        CarsInfo.UsedCarsInfo OBJ = new CarsInfo.UsedCarsInfo();



        string sort = string.Empty;


        objCarsFilter.CarMakeid = carMake;
        objCarsFilter.CarModalId = CarModal;
        objCarsFilter.Orderby = Orderby;
        objCarsFilter.ZipCode = Zipcode;

        switch (Mileage)
        {
            case "Mileage1":
                objCarsFilter.Mileage1 = "Mileage1";
                break;
            case "Mileage2":
                objCarsFilter.Mileage2 = "Mileage2";
                break;
            case "Mileage3":
                objCarsFilter.Mileage3 = "Mileage3";
                break;
            case "Mileage4":
                objCarsFilter.Mileage4 = "Mileage4";
                break;
            case "Mileage5":
                objCarsFilter.Mileage5 = "Mileage5";
                break;
            case "Mileage6":
                objCarsFilter.Mileage6 = "Mileage6";
                break;
            case "Mileage7":
                objCarsFilter.Mileage7 = "Mileage7";
                break;
        }
        switch (Year)
        {
            case "Year1a":
                objCarsFilter.Year1a = "Year1a";
                break;
            case "Year1b":
                objCarsFilter.Year1b = "Year1b";
                break;
            case "Year1":
                objCarsFilter.Year1 = "Year1";
                break;
            case "Year2":
                objCarsFilter.Year2 = "Year2";
                break;
            case "Year3":
                objCarsFilter.Year3 = "Year3";
                break;
            case "Year4":
                objCarsFilter.Year4 = "Year4";
                break;
            case "Year5":
                objCarsFilter.Year5 = "Year5";
                break;
            case "Year6":
                objCarsFilter.Year6 = "Year6";
                break;
            case "Year7":
                objCarsFilter.Year7 = "Year7";
                break;
        }
        switch (Price)
        {
            case "Price1":
                objCarsFilter.Price1 = "Price1";
                break;
            case "Price2":
                objCarsFilter.Price2 = "Price2";
                break;
            case "Price3":
                objCarsFilter.Price3 = "Price3";
                break;
            case "Price4":
                objCarsFilter.Price4 = "Price4";
                break;
            case "Price5":
                objCarsFilter.Price5 = "Price5";
                break;
        };


        NotificationBL objFilterCars = new NotificationBL();
        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            string parameter = carMake + "," + CarModal + "," + Mileage + "," + Year + "," + Price + "," + Orderby + "," + Zipcode;
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("MobileCarsNotificationSearch", CustomerID, AuthenticationID, parameter);
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            objFilterdata = (List<CarsInfo.UsedCarsInfo>)objFilterCars.GetMobileNotificationSearch(objCarsFilter, exteriorColor.Trim(), interiorColor.Trim(), Transmission.Trim(), VehiceCondition.Trim(),
                              DriveTrain.Trim(), numberOfCylinder.Trim(), numberOfDoors.Trim(), fuelType.Trim());

        }
        return objFilterdata;

    }

  
}
