using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CarsInfo;
using System.ServiceModel.Web;
using System.Collections;
using System.ServiceModel.Activation;
using System.IO;

// NOTE: If you change the interface name "IServiceMobile" here, you must also update the reference to "IServiceMobile" in Web.config.
[ServiceContract(Namespace = "http://microsoft.wcf.documentation", SessionMode = SessionMode.Allowed)]

public interface IServiceMobile
{
    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/PerformLoginMobile/{Username}/{Password}/{AuthenticationID}/{CustomerID}/",
    BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.UserLoginInfo> PerformLoginMobile(string Username, string Password, string AuthenticationID, string CustomerID);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/PerformLogoutMobile/{UserID}/{SessionID}/{AuthenticationID}/{CustomerID}/",
    BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    string PerformLogoutMobile(string UserID, string SessionID, string AuthenticationID, string CustomerID);


    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetUserRegistrationDetailsByID/{UID}/{AuthenticationID}/{CustomerID}/{SessionID}",
      BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.MobileUserRegData> GetUserRegistrationDetailsByID(string UID, string AuthenticationID, string CustomerID, string SessionID);


    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/SalesAgentLogin/{Username}/{Password}/{CenterCode}/",
      BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.SalesInfo> SalesAgentLogin(string Username, string Password, string CenterCode);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetRecentCarsMobile/{sCurrentPage}/{PageSize}/{Orderby}/{Sort}/{sPin}/{AuthenticationID}/{CustomerID}/",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<UsedCarsInfo> GetRecentCarsMobile(string sCurrentPage, string PageSize, string Orderby, string Sort, string sPin, string AuthenticationID, string CustomerID);


    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/SaveCallRequestMobile/{BuyerPhoneNo}/{CarID}/{CustomerPhoneNo}/{AuthenticationID}/{CustomerID}/",
    BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    bool SaveCallRequestMobile(string BuyerPhoneNo, string CarID, string CustomerPhoneNo, string AuthenticationID, string CustomerID);



    [OperationContract]
    [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/SaveBuyerRequestMobile/{BuyerEmail}/{BuyerCity}/{BuyerPhone}/{BuyerFirstName}/{BuyerLastName}/{BuyerComments}/{IpAddress}/{Sellerphone}/{Sellerprice}/{Carid}/{sYear}/{Make}/{Model}/{price}/{ToEmail}/{AuthenticationID}/{CustomerID}/",
    BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    bool SaveBuyerRequestMobile(string BuyerEmail, string BuyerCity, string BuyerPhone, string BuyerFirstName, string BuyerLastName,
        string BuyerComments, string IpAddress, string Sellerphone, string Sellerprice, string Carid, string sYear, string Make,
        string Model, string price, string ToEmail, string AuthenticationID, string CustomerID);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetCarFeatures/{sCarId}/{AuthenticationID}/{CustomerID}/",
    BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    ArrayList GetCarFeatures(string sCarId, string AuthenticationID, string CustomerID);


    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetCarsSearchJSON/{carMakeid}/{CarModalId}/{ZipCode}/{WithinZip}/{pageNo}/{pageresultscount}/{orderby}/{AuthenticationID}/{CustomerID}/",
        BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.UsedCarsInfo> GetCarsSearchJSON(string carMakeid, string CarModalId, string ZipCode, string WithinZip, string pageNo, string pageresultscount, string orderby, string AuthenticationID, string CustomerID);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetMakes/{AuthenticationID}/{CustomerID}/", BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<MakesInfo> GetMakes(string AuthenticationID, string CustomerID);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetModelsInfo/{AuthenticationID}/{CustomerID}/",
        BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<ModelsInfo> GetModelsInfo(string AuthenticationID, string CustomerID);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetCarsFilterMobile/{carMakeid}/{CarModalId}/{Mileage}/{Year}/{Price}/{Sort}/{Orderby}/{pageSize}/{CurrentPage}/{Zipcode}/{AuthenticationID}/{CustomerID}/",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.UsedCarsInfo> GetCarsFilterMobile(string carMakeID, string CarModalId, string Mileage, string Year, string Price, string Sort, string Orderby, string pageSize, string CurrentPage, string Zipcode, string AuthenticationID, string CustomerID);


    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetCarsFilterAndroidMobile/{carMake}/{CarModal}/{Mileage}/{Year}/{Price}/{Sort}/{Orderby}/{pageSize}/{CurrentPage}/{Zipcode}/{AuthenticationID}/{CustomerID}/",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.UsedCarsInfo> GetCarsFilterAndroidMobile(string carMake, string CarModal, string Mileage, string Year, string Price, string Sort, string Orderby, string pageSize, string CurrentPage, string Zipcode, string AuthenticationID, string CustomerID);


    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/FindCarID/{sCarid}/{AuthenticationID}/{CustomerID}/",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.UsedCarsInfo> FindCarID(string sCarid, string AuthenticationID, string CustomerID);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetPackageDetailsByUID/{UID}/{AuthenticationID}/{CustomerID}/{SessionID}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.PackagesInfo> GetPackageDetailsByUID(string UID, string AuthenticationID, string CustomerID,string SessionID);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/SendMobileRegistrationRequest/{Name}/{Phonenumber}/{Email}/{AuthenticationID}/{CustomerID}/",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    bool SendMobileRegistrationRequest(string Name, string Phonenumber, string Email, string AuthenticationID, string CustomerID);


    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateUserRegistration/{name}/{address}/{city}/{stateID}/{zip}/{phone}/{UID}/{businessName}/{altEmail}/{altPhone}/{AuthenticationID}/{CustomerID}/{SessionID}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
     List<UserRegistrationInfo> UpdateUserRegistration(string name, string address, string city, string stateID, string zip, string phone, string UID,
                                                             string businessName, string altEmail, string altPhone,string AuthenticationID,string CustomerID,string SessionID);
    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateSellerInformation/{sellerID}/{sellerName}/{city}/{state}/{zip}/{phone}/{email}/{carID}/{UID}/{AuthenticationID}/{CustomerID}/{SessionID}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    string UpdateSellerInformation(string sellerID, string sellerName,string city, string state, string zip, string phone, string email, string carID, string UID, string AuthenticationID, string CustomerID, string SessionID);

   

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateMobileCarStatusByCarID/{CarID}/{UID}/{AdstatusName}/{AuthenticationID}/{CustomerID}/{SessionID}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    string UpdateMobileCarStatusByCarID(string CarID, string UID, string AdstatusName, string AuthenticationID, string CustomerID, string SessionID);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/FindMobileCarsByUID/{UID}/{AuthenticationID}/{CustomerID}/",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.UsedCarsInfo> FindMobileCarsByUID(string UID, string AuthenticationID, string CustomerID);

   

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetMultisiteListingsByCarID/{CarID}/{AuthenticationID}/{CustomerID}/{SessionID}/{UID}/",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.MultisiteInfo> GetMultisiteListingsByCarID(string CarID, string AuthenticationID, string CustomerID, string SessionID, string UID);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetMakeCounts/{AuthenticationID}/{CustomerID}/",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<MakeCountInfo> GetMakeCounts(string AuthenticationID, string CustomerID);

    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/MobileCarsNotificationSearch/{carMake}/{CarModal}/{Mileage}/{Year}/{Price}/{Orderby}/{Zipcode}/{exteriorColor}/{interiorColor}/{Transmission}/{VehiceCondition}/{DriveTrain}/{numberOfCylinder}/{numberOfDoors}/{fuelType}/{AuthenticationID}/{CustomerID}",
            BodyStyle = WebMessageBodyStyle.WrappedResponse)]
    List<CarsInfo.UsedCarsInfo> MobileCarsNotificationSearch(string carMake, string CarModal,
                                 string Mileage, string Year, string Price, string Orderby, string Zipcode, string exteriorColor, string interiorColor,
           string Transmission, string VehiceCondition, string DriveTrain, string numberOfCylinder, string numberOfDoors, string fuelType, string AuthenticationID, string CustomerID);
}
