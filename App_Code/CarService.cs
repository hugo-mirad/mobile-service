
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
using System.Configuration;
//using CarsBL;


#endregion System References
//using CarsInfo;

#region Application References
using CarsInfo;
using CarsBL;
using CarsBL.Transactions;
using CarsBL.Masters;
using System.Web.Script.Services;
using System.Runtime.Serialization.Json;
using System.IO;
#endregion Application References
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

/// <summary>
/// Summary description for CarService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.ComponentModel.ToolboxItem(false)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class CarService : System.Web.Services.WebService
{

    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public List<CarsInfo.UCEDealerInfo> UploadCarDetailsFromDealerSite(CarsInfo.DealerSiteCarInfo objDealerCaInfo, string Action)
    {

        var obj = new List<CarsInfo.UCEDealerInfo>();
        try
        {
            CarsBL.DealerSiteBL.UploadCarDealerSiteBL objDealerCar = new CarsBL.DealerSiteBL.UploadCarDealerSiteBL();
            if (Action == "Upload")
            {
                obj = (List<CarsInfo.UCEDealerInfo>)objDealerCar.UploadCar(objDealerCaInfo);
            }
            else
            {
                obj = (List<CarsInfo.UCEDealerInfo>)objDealerCar.UpdateCar(objDealerCaInfo);
            }
        }
        catch (Exception ex)
        {
        }
        return obj;
    }

    //Upload carFeatures from DealerSite to UCE ******(shobha)****
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public string UploadCarFeaturesFromDealerSite(string CarID, string FeatureNmae, string FeatureTypeName, string Isactive, string UID)
    {
        string sCarID = string.Empty;
        DataSet dsCarID = new DataSet();
        try
        {
            CarsBL.DealerSiteBL.UploadCarDealerSiteBL objDealerCar = new CarsBL.DealerSiteBL.UploadCarDealerSiteBL();
            dsCarID = objDealerCar.UploadCarFeatures(CarID, FeatureNmae, FeatureTypeName, Isactive, UID);
            if (dsCarID.Tables.Count > 0)
            {
                if (dsCarID.Tables[0].Rows.Count > 0)
                {
                    sCarID = dsCarID.Tables[0].Rows[0]["CarID"].ToString();
                }
            }
        }
        catch (Exception ex)
        {
        }
        return sCarID;

    }

    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public void UploadCarPictures(string PicFullPath, string CarID, string Make, string Model, string Year, string CarNum, string UID)
    {
        try
        {
            CarsBL.DealerSiteBL.UploadCarDealerSiteBL objCar = new CarsBL.DealerSiteBL.UploadCarDealerSiteBL();
            int Carnum = Convert.ToInt32(CarNum);
            string picNeLocation = "CarService/CarImages/" + Year + "/" + Make + "/" + Model + "/" + CarID;
            string PicFileNewName = Year + "_" + Make + "_" + Model + "_" + CarID + Carnum + ".jpg";
            string localPath = Server.MapPath("~/" + picNeLocation + "/");

            if (System.IO.Directory.Exists(localPath) == false)
            {
                System.IO.Directory.CreateDirectory(localPath);
            }

            string NewPicFileFullPath = localPath + PicFileNewName;
            //string localpath = "E:\\Jagadesh\\j.jpg";
            using (WebClient Client = new WebClient())
            {
                Client.DownloadFile(PicFullPath, NewPicFileFullPath);
                // Client.DownloadFile(
            }

            objCar.SaveCarPicturesFromDealerSite(Convert.ToInt32(CarID), picNeLocation, PicFileNewName, Convert.ToInt32(UID));
        }
        catch (Exception ex)
        {
            while (ex != null)
            {
                Console.WriteLine(ex.Message);
                ex = ex.InnerException;
            }
        }
    }

    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    [WebMethod(EnableSession = true)]
    public bool CheckZips(string zipId, string AuthenticationID, string CustomerID)
    {
        ZipCodesBL objZipCodesBL = new ZipCodesBL();

        List<ZipcodeDistancesInfo> objZipCode = new List<ZipcodeDistancesInfo>();
        MobileBL objMobileBL = new MobileBL();
        if (CustomerID != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("CheckZips", CustomerID, AuthenticationID,zipId);
        }
        if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
        {
            objZipCode = (List<ZipcodeDistancesInfo>)objZipCodesBL.GetZips(zipId);
           
        }

        if (objZipCode.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public string UploadPictureByCarID(string CarID, string make, string model, string year, string UserID, byte[] picContent, string AuthenticationID, string CustomerID, string SessionID)
    {

        string bStatus = "Failed";
        MobileBL objMobile = new MobileBL();
        UsedCarsInfo objCarPicInfo = new UsedCarsInfo();

        try
        {
            if (CustomerID != "")
            {
                string parameters=CarID+","+make+","+model+","+year+","+UserID;
                DataSet dsSaveCustInfo = objMobile.SaveMobileCustomerInfo("UploadPictureByCarID", CustomerID, AuthenticationID,parameters);
            }
            bool bnew = objMobile.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UserID));
            if (bnew)
            {
                try
                {
                    if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
                    {
                        objCarPicInfo.Carid = Convert.ToInt32(CarID);

                        MemoryStream ms = new MemoryStream(picContent);

                        System.Drawing.Bitmap oBitmap1 = new System.Drawing.Bitmap(ms);

                        DataSet dsImagesData = objMobile.GetMobileCarPicIDs(Convert.ToInt32(CarID));

                        string FileNameFullLocation = "CarImages" + "/" + year.ToString() + "/" + make.ToString() + "/" + model + "/";
                        string FileNameFullThumb = "CarImages" + "/" + year.ToString() + "/" + make.ToString() + "/" + model + "/";
                        string FileLocatinon = "MobileService/CarImages" + "/" + year.ToString() + "/" + make.ToString() + "/" + model + "/";
                        string FileName = string.Empty;
                        if (dsImagesData.Tables.Count > 0)
                        {
                            if (dsImagesData.Tables[0].Rows.Count > 0)
                            {
                                objCarPicInfo.PIC0 = dsImagesData.Tables[0].Rows[0]["pic0"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic0"].ToString();
                                objCarPicInfo.PIC1 = dsImagesData.Tables[0].Rows[0]["pic1"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic1"].ToString();
                                objCarPicInfo.PIC2 = dsImagesData.Tables[0].Rows[0]["pic2"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic2"].ToString();
                                objCarPicInfo.PIC3 = dsImagesData.Tables[0].Rows[0]["pic3"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic3"].ToString();
                                objCarPicInfo.PIC4 = dsImagesData.Tables[0].Rows[0]["pic4"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic4"].ToString();
                                objCarPicInfo.PIC5 = dsImagesData.Tables[0].Rows[0]["pic5"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic5"].ToString();
                                objCarPicInfo.PIC6 = dsImagesData.Tables[0].Rows[0]["pic6"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic6"].ToString();
                                objCarPicInfo.PIC7 = dsImagesData.Tables[0].Rows[0]["pic7"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic7"].ToString();
                                objCarPicInfo.PIC8 = dsImagesData.Tables[0].Rows[0]["pic8"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic8"].ToString();
                                objCarPicInfo.PIC9 = dsImagesData.Tables[0].Rows[0]["pic9"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic9"].ToString();
                                objCarPicInfo.PIC10 = dsImagesData.Tables[0].Rows[0]["pic10"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic10"].ToString();
                                objCarPicInfo.PIC11 = dsImagesData.Tables[0].Rows[0]["pic11"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic11"].ToString();
                                objCarPicInfo.PIC12 = dsImagesData.Tables[0].Rows[0]["pic12"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic12"].ToString();
                                objCarPicInfo.PIC13 = dsImagesData.Tables[0].Rows[0]["pic13"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic13"].ToString();
                                objCarPicInfo.PIC14 = dsImagesData.Tables[0].Rows[0]["pic14"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic14"].ToString();
                                objCarPicInfo.PIC15 = dsImagesData.Tables[0].Rows[0]["pic15"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic15"].ToString();
                                objCarPicInfo.PIC16 = dsImagesData.Tables[0].Rows[0]["pic16"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic16"].ToString();
                                objCarPicInfo.PIC17 = dsImagesData.Tables[0].Rows[0]["pic17"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic17"].ToString();
                                objCarPicInfo.PIC18 = dsImagesData.Tables[0].Rows[0]["pic18"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic18"].ToString();
                                objCarPicInfo.PIC19 = dsImagesData.Tables[0].Rows[0]["pic19"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic19"].ToString();
                                objCarPicInfo.PIC20 = dsImagesData.Tables[0].Rows[0]["pic20"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic20"].ToString();


                                //******************Creating bigImage***************************

                                if (objCarPicInfo.PIC1 == "0" || objCarPicInfo.PIC1 == null || objCarPicInfo.PIC1 == "" || objCarPicInfo.PIC1 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image1.Jpeg";
                                }
                                else if (objCarPicInfo.PIC2 == "0" || objCarPicInfo.PIC2 == null || objCarPicInfo.PIC2 == "" || objCarPicInfo.PIC2 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image2.Jpeg";
                                }
                                else if (objCarPicInfo.PIC3 == "0" || objCarPicInfo.PIC3 == null || objCarPicInfo.PIC3 == "" || objCarPicInfo.PIC3 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image3.Jpeg";
                                }
                                else if (objCarPicInfo.PIC4 == "0" || objCarPicInfo.PIC4 == null || objCarPicInfo.PIC4 == "" || objCarPicInfo.PIC4 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image4.Jpeg";
                                }
                                else if (objCarPicInfo.PIC5 == "0" || objCarPicInfo.PIC5 == null || objCarPicInfo.PIC5 == "" || objCarPicInfo.PIC5 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image5.Jpeg";
                                }
                                else if (objCarPicInfo.PIC6 == "0" || objCarPicInfo.PIC6 == null || objCarPicInfo.PIC6 == "" || objCarPicInfo.PIC6 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image6.Jpeg";
                                }
                                else if (objCarPicInfo.PIC7 == "0" || objCarPicInfo.PIC7 == null || objCarPicInfo.PIC7 == "" || objCarPicInfo.PIC7 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image7.Jpeg";
                                }
                                else if (objCarPicInfo.PIC8 == "0" || objCarPicInfo.PIC8 == null || objCarPicInfo.PIC8 == "" || objCarPicInfo.PIC8 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image8.Jpeg";
                                }
                                else if (objCarPicInfo.PIC9 == "0" || objCarPicInfo.PIC9 == null || objCarPicInfo.PIC9 == "" || objCarPicInfo.PIC9 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image9.Jpeg";
                                }

                                else if (objCarPicInfo.PIC10 == "0" || objCarPicInfo.PIC10 == null || objCarPicInfo.PIC10 == "" || objCarPicInfo.PIC10 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image10.Jpeg";
                                }
                                else if (objCarPicInfo.PIC11 == "0" || objCarPicInfo.PIC11 == null || objCarPicInfo.PIC11 == "" || objCarPicInfo.PIC11 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image11.Jpeg";
                                }
                                else if (objCarPicInfo.PIC12 == "0" || objCarPicInfo.PIC12 == null || objCarPicInfo.PIC12 == "" || objCarPicInfo.PIC12 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image12.Jpeg";
                                }
                                else if (objCarPicInfo.PIC13 == "0" || objCarPicInfo.PIC13 == null || objCarPicInfo.PIC13 == "" || objCarPicInfo.PIC13 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image13.Jpeg";
                                }
                                else if (objCarPicInfo.PIC14 == "0" || objCarPicInfo.PIC14 == null || objCarPicInfo.PIC14 == "" || objCarPicInfo.PIC14 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image14.Jpeg";
                                }
                                else if (objCarPicInfo.PIC15 == "0" || objCarPicInfo.PIC15 == null || objCarPicInfo.PIC15 == "" || objCarPicInfo.PIC15 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image15.Jpeg";
                                }
                                else if (objCarPicInfo.PIC16 == "0" || objCarPicInfo.PIC16 == null || objCarPicInfo.PIC16 == "" || objCarPicInfo.PIC16 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image16.Jpeg";
                                }
                                else if (objCarPicInfo.PIC17 == "0" || objCarPicInfo.PIC17 == null || objCarPicInfo.PIC17 == "" || objCarPicInfo.PIC17 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image17.Jpeg";
                                }
                                else if (objCarPicInfo.PIC18 == "0" || objCarPicInfo.PIC18 == null || objCarPicInfo.PIC18 == "" || objCarPicInfo.PIC18 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image18.Jpeg";
                                }
                                else if (objCarPicInfo.PIC19 == "0" || objCarPicInfo.PIC19 == null || objCarPicInfo.PIC19 == "" || objCarPicInfo.PIC19 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image19.Jpeg";
                                }
                                else if (objCarPicInfo.PIC20 == "0" || objCarPicInfo.PIC20 == null || objCarPicInfo.PIC20 == "" || objCarPicInfo.PIC20 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image20.Jpeg";
                                }



                                string sFilePath = Server.MapPath(FileNameFullLocation);
                                if (System.IO.Directory.Exists(sFilePath) == false)
                                {
                                    System.IO.Directory.CreateDirectory(sFilePath);
                                }

                                Graphics oGraphic1 = default(Graphics);

                                int newwidthimg1 = 600;
                                // Here create a new bitmap object of the same height and width of the image.
                                float AspectRatio = (float)oBitmap1.Size.Width / (float)oBitmap1.Size.Height;
                                int newHeight1 = Convert.ToInt32(newwidthimg1 / AspectRatio);

                                Bitmap bmpNew1 = new Bitmap(newwidthimg1, newHeight1);
                                oGraphic1 = Graphics.FromImage(bmpNew1);

                                oGraphic1.CompositingQuality = CompositingQuality.HighQuality;
                                oGraphic1.SmoothingMode = SmoothingMode.HighQuality;
                                oGraphic1.InterpolationMode = InterpolationMode.HighQualityBicubic;


                                oGraphic1.DrawImage(oBitmap1, new Rectangle(0, 0, bmpNew1.Width, bmpNew1.Height), 0, 0, oBitmap1.Width, oBitmap1.Height, GraphicsUnit.Pixel);
                                // Release the lock on the image file. Of course,
                                // image from the image file is existing in Graphics object
                                oBitmap1.Dispose();
                                oBitmap1 = bmpNew1;

                                oBitmap1.Save(sFilePath + "/" + FileName, ImageFormat.Jpeg);

                                oBitmap1.Dispose();


                                string picID = objMobile.SaveMobileCarPicture(FileLocatinon, "Jpeg", FileName, Convert.ToInt32(UserID));


                                if (objCarPicInfo.PIC1 == "0" || objCarPicInfo.PIC1 == null || objCarPicInfo.PIC1 == "" || objCarPicInfo.PIC1 == " ")
                                {
                                    objCarPicInfo.PIC1 = picID;
                                }
                                else if (objCarPicInfo.PIC2 == "0" || objCarPicInfo.PIC2 == null || objCarPicInfo.PIC2 == "" || objCarPicInfo.PIC2 == " ")
                                {
                                    objCarPicInfo.PIC2 = picID;
                                }
                                else if (objCarPicInfo.PIC3 == "0" || objCarPicInfo.PIC3 == null || objCarPicInfo.PIC3 == "" || objCarPicInfo.PIC3 == " ")
                                {
                                    objCarPicInfo.PIC3 = picID;
                                }
                                else if (objCarPicInfo.PIC4 == "0" || objCarPicInfo.PIC4 == null || objCarPicInfo.PIC4 == "" || objCarPicInfo.PIC4 == " ")
                                {
                                    objCarPicInfo.PIC4 = picID;
                                }
                                else if (objCarPicInfo.PIC5 == "0" || objCarPicInfo.PIC5 == null || objCarPicInfo.PIC5 == "" || objCarPicInfo.PIC5 == " ")
                                {
                                    objCarPicInfo.PIC5 = picID;
                                }
                                else if (objCarPicInfo.PIC6 == "0" || objCarPicInfo.PIC6 == null || objCarPicInfo.PIC6 == "" || objCarPicInfo.PIC6 == " ")
                                {
                                    objCarPicInfo.PIC6 = picID;
                                }
                                else if (objCarPicInfo.PIC7 == "0" || objCarPicInfo.PIC7 == null || objCarPicInfo.PIC7 == "" || objCarPicInfo.PIC7 == " ")
                                {
                                    objCarPicInfo.PIC7 = picID;
                                }
                                else if (objCarPicInfo.PIC8 == "0" || objCarPicInfo.PIC8 == null || objCarPicInfo.PIC8 == "" || objCarPicInfo.PIC8 == " ")
                                {
                                    objCarPicInfo.PIC8 = picID;
                                }
                                else if (objCarPicInfo.PIC9 == "0" || objCarPicInfo.PIC9 == null || objCarPicInfo.PIC9 == "" || objCarPicInfo.PIC9 == " ")
                                {
                                    objCarPicInfo.PIC9 = picID;
                                }

                                else if (objCarPicInfo.PIC10 == "0" || objCarPicInfo.PIC10 == null || objCarPicInfo.PIC10 == "" || objCarPicInfo.PIC10 == " ")
                                {
                                    objCarPicInfo.PIC10 = picID;
                                }
                                else if (objCarPicInfo.PIC11 == "0" || objCarPicInfo.PIC11 == null || objCarPicInfo.PIC11 == "" || objCarPicInfo.PIC11 == " ")
                                {
                                    objCarPicInfo.PIC11 = picID;
                                }
                                else if (objCarPicInfo.PIC12 == "0" || objCarPicInfo.PIC12 == null || objCarPicInfo.PIC12 == "" || objCarPicInfo.PIC12 == " ")
                                {
                                    objCarPicInfo.PIC12 = picID;
                                }
                                else if (objCarPicInfo.PIC13 == "0" || objCarPicInfo.PIC13 == null || objCarPicInfo.PIC13 == "" || objCarPicInfo.PIC13 == " ")
                                {
                                    objCarPicInfo.PIC13 = picID;
                                }
                                else if (objCarPicInfo.PIC14 == "0" || objCarPicInfo.PIC14 == null || objCarPicInfo.PIC14 == "" || objCarPicInfo.PIC14 == " ")
                                {
                                    objCarPicInfo.PIC14 = picID;
                                }
                                else if (objCarPicInfo.PIC15 == "0" || objCarPicInfo.PIC15 == null || objCarPicInfo.PIC15 == "" || objCarPicInfo.PIC15 == " ")
                                {
                                    objCarPicInfo.PIC15 = picID;
                                }
                                else if (objCarPicInfo.PIC16 == "0" || objCarPicInfo.PIC16 == null || objCarPicInfo.PIC16 == "" || objCarPicInfo.PIC16 == " ")
                                {
                                    objCarPicInfo.PIC16 = picID;
                                }
                                else if (objCarPicInfo.PIC17 == "0" || objCarPicInfo.PIC17 == null || objCarPicInfo.PIC17 == "" || objCarPicInfo.PIC17 == " ")
                                {
                                    objCarPicInfo.PIC17 = picID;
                                }
                                else if (objCarPicInfo.PIC18 == "0" || objCarPicInfo.PIC18 == null || objCarPicInfo.PIC18 == "" || objCarPicInfo.PIC18 == " ")
                                {
                                    objCarPicInfo.PIC18 = picID;
                                }
                                else if (objCarPicInfo.PIC19 == "0" || objCarPicInfo.PIC19 == null || objCarPicInfo.PIC19 == "" || objCarPicInfo.PIC19 == " ")
                                {
                                    objCarPicInfo.PIC19 = picID;
                                }
                                else if (objCarPicInfo.PIC20 == "0" || objCarPicInfo.PIC20 == null || objCarPicInfo.PIC20 == "" || objCarPicInfo.PIC20 == " ")
                                {
                                    objCarPicInfo.PIC20 = picID;
                                }


                                if (objCarPicInfo.PIC0 == "0" || objCarPicInfo.PIC0 == null || objCarPicInfo.PIC0 == "" || objCarPicInfo.PIC0 == " ")
                                {

                                    System.Drawing.Bitmap oBitmap = new System.Drawing.Bitmap(ms);

                                    string sFilePath1 = Server.MapPath(FileNameFullThumb);
                                    string FileNameThumb = year.ToString() + "_" + make + "_" + model + "_" + CarID + "Thumb.Jpeg";
                                    if (System.IO.Directory.Exists(sFilePath1) == false)
                                    {
                                        System.IO.Directory.CreateDirectory(sFilePath1);
                                    }

                                    Graphics oGraphic = default(Graphics);

                                    int newwidthimg = 250;
                                    // Here create a new bitmap object of the same height and width of the image.
                                    float AspectRatio1 = (float)oBitmap.Size.Width / (float)oBitmap.Size.Height;
                                    int newHeight = Convert.ToInt32(newwidthimg / AspectRatio);

                                    Bitmap bmpNew = new Bitmap(newwidthimg, newHeight);
                                    oGraphic = Graphics.FromImage(bmpNew);

                                    oGraphic.CompositingQuality = CompositingQuality.HighQuality;
                                    oGraphic.SmoothingMode = SmoothingMode.HighQuality;
                                    oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;


                                    oGraphic.DrawImage(oBitmap, new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), 0, 0, oBitmap.Width, oBitmap.Height, GraphicsUnit.Pixel);
                                    // Release the lock on the image file. Of course,
                                    // image from the image file is existing in Graphics object
                                    oBitmap.Dispose();
                                    oBitmap = bmpNew;

                                    oBitmap.Save(sFilePath1 + "/" + FileNameThumb, ImageFormat.Jpeg);

                                    oBitmap.Dispose();


                                    string picIDs = objMobile.SaveMobileCarPicture(FileLocatinon, "Jpeg", FileNameThumb, Convert.ToInt32(UserID));
                                    objCarPicInfo.PIC0 = picIDs;
                                }

                                bool bnw = objMobile.UpdateMobilePicturesByCarId(objCarPicInfo);
                                if (bnw)
                                {
                                    bStatus = "Success";
                                }

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }
        catch (Exception ex)
        {
        }


        return bStatus;
    }

    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public string UpdateCarFeatures(string CarID,string UID,int[] Features, string AuthenticationID, string CustomerID,string SessionID)
    {
        string returnStatus = "Failed";
        MobileBL objMobile = new MobileBL();
        DataSet ds=new DataSet();
        try
        {
          if (CustomerID != "")
          {
              string parameters = CarID + "," + UID;
            DataSet dsSaveCustInfo = objMobile.SaveMobileCustomerInfo("UpdateCarFeatures", CustomerID, AuthenticationID,parameters);
          }
          bool bnew = objMobile.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
          if (bnew)
          {
              if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
              {
                  for (int i = 0; i < Features.Length; i++)
                  {
                          int FeatureID = Features[i] / 10;
                          int IsActive = Features[i] % 10;
                          ds = objMobile.mobileUpdateCarfeatures(Convert.ToInt32(CarID), FeatureID, IsActive, Convert.ToInt32(UID));
                          returnStatus = "Success";
                  }
              }
          }
          else
          {
              returnStatus = "Session timed out";
          }
        }
        catch (Exception ex)
        {
        }
       return returnStatus;
    }

    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public string UpdateCarDetails(string UID, string Year, string ExteriorColor, string InteriorColor, string Transmission, string DriveTrain, string NumberOfDoors, string MakeModelID,
                                    string BodyTypeID, string CarID, string Price, string Mileage, string VIN, string NumberOfCylinder, string FueltypeID, string zip,
                                    string City, string Description, string VehicleCondition, string Title, string StateID, string AuthenticationID, string CustomerID, string SessionID)
    {

        string returnCarID = "Failed";
        DropdownBL objdropdownBL = new DropdownBL();
        CarsInfo.CarsInfo objcarsInfo = new CarsInfo.CarsInfo();

        MobileBL objMobileBL = new MobileBL();
        if (CustomerID.Trim() != "")
        {
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("UpdateCarDetails", CustomerID, AuthenticationID,CarID);
        }
        bool bnew = objMobileBL.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
        if (bnew)
        {
            try
            {
                if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
                {
                    int Uid = Convert.ToInt32(UID);
                    objcarsInfo.YearOfMake = Convert.ToInt32(Year);
                    objcarsInfo.MakeModelID = Convert.ToInt32(MakeModelID);
                    objcarsInfo.BodyTypeID = Convert.ToInt32(BodyTypeID);
                    objcarsInfo.CarID = Convert.ToInt32(CarID);
                    //Check for same make,model year of car
                    DataSet dsCarID = objMobileBL.CheckMobileCarDetailsByCarID(Convert.ToInt32(CarID), objcarsInfo.MakeModelID, objcarsInfo.YearOfMake);
                    if (dsCarID.Tables.Count > 0)
                    {
                        if (dsCarID.Tables[0].Rows.Count > 0)
                        {
                            if (Price == "")
                            {
                                objcarsInfo.Price = "0";
                            }
                            else
                            {
                                objcarsInfo.Price = Price;
                            }
                            if (Mileage == "")
                            {
                                objcarsInfo.Mileage = "0";
                            }
                            else
                            {
                                objcarsInfo.Mileage = Mileage;
                            }
                            objcarsInfo.ExteriorColor = ExteriorColor;
                            objcarsInfo.InteriorColor = InteriorColor;
                            objcarsInfo.Transmission = Transmission;
                            objcarsInfo.DriveTrain = DriveTrain;
                            objcarsInfo.NumberOfDoors = NumberOfDoors;
                            objcarsInfo.VIN = VIN;
                            objcarsInfo.NumberOfCylinder = NumberOfCylinder;
                            objcarsInfo.FuelTypeID = Convert.ToInt32(FueltypeID);
                            if (zip.Length == 4)
                            {
                                objcarsInfo.Zipcode = "0" + zip;
                            }
                            else
                            {
                                objcarsInfo.Zipcode = zip;
                            }
                            objcarsInfo.City = GeneralFunc.ToProper(City);
                            objcarsInfo.StateID = Convert.ToInt32(StateID);
                            string Condition = GeneralFunc.ToProper(Description);
                            DataSet dsCarsDetails = objdropdownBL.USP_SaveCarDetails(objcarsInfo, Condition, VehicleCondition, Title, Uid);
                            if (dsCarsDetails.Tables.Count > 0)
                            {
                                if (dsCarsDetails.Tables[0].Rows.Count > 0)
                                {
                                    returnCarID = "Success";
                                }
                                else
                                 {
                                    returnCarID = "Failed";
                                }
                            }
                            else
                            {
                                returnCarID = "Failed";
                            }
                        }
                        else
                        {
                            returnCarID = "Sorry we are not able to change year,make and model of your car";
                        }
                    }
                    else
                    {
                        returnCarID = "Sorry we are not able to change year,make and model of your car";
                    }
                }
                
            }

            catch (Exception ex)
            {
            }
        }
        else
        {
            returnCarID = "Session timed out";
        }
        return returnCarID;
    }

    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public List<UserRegistrationInfo> UpdateUserRegistration(string name, string address, string city, string stateID, string zip, string phone, string UID,
                                                            string businessName, string altEmail, string altPhone, string AuthenticationID, string CustomerID, string SessionID)
    {
        MobileBL objUserRegBL = new MobileBL();
        var obj = new List<UserRegistrationInfo>();
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




    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public List<CarsInfo.MobileCarInfo> AddCarDetails(string make, string model, string price, string year, string mileage, string UID, string userPackID, string packageID, string AuthenticationID, string CustomerID, string SessionID)
    {
        List<CarsInfo.MobileCarInfo> obj = new List<CarsInfo.MobileCarInfo>();
        MobileBL objMobileBL = new MobileBL();
        MobileCarInfo objCarInfo = new MobileCarInfo();
        if (CustomerID.Trim() != "")
        {
            string parameter = make + "," + model + "," + price + "," + year;
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("AddCarDetails", CustomerID, AuthenticationID, parameter);
        }
        try
        {
            bool bnew = objMobileBL.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
            if (bnew)
            {
                DataSet dsPackages = objMobileBL.MobileChkPackageForAddCar(Convert.ToInt32(UID));

                if (dsPackages.Tables.Count > 0)
                {
                    if (dsPackages.Tables[0].Rows.Count > 0)
                    {
                        if (make != "" && model != "" && year != "" && price != "" && mileage != "")
                        {
                            DataSet dsCar = objMobileBL.SaveMobileNewCarDetails(make, model, year, mileage, price, Convert.ToInt32(userPackID), Convert.ToInt32(packageID), Convert.ToInt32(UID));
                            if (dsCar.Tables.Count > 0)
                            {
                                if (dsCar.Tables[0].Rows.Count > 0)
                                {
                                    objCarInfo.AASuccess = "Success";
                                    objCarInfo.CarID = dsCar.Tables[0].Rows[0]["CarID"].ToString();
                                }
                                else
                                {
                                    objCarInfo.AASuccess = "Failure";
                                    objCarInfo.Reason = "Car details are not added";
                                }
                            }
                            else
                            {
                                objCarInfo.AASuccess = "Failure";
                                objCarInfo.Reason = "Car details are not added";
                            }

                        }
                        else if (make == "")
                        {
                            objCarInfo.AASuccess = "Failure";
                            objCarInfo.Reason = "Make should not be empty";

                        }
                        else if (model == "")
                        {
                            objCarInfo.AASuccess = "Failure";
                            objCarInfo.Reason = "Model should not be empty";

                        }
                        else if (price == "")
                        {
                            objCarInfo.AASuccess = "Failure";
                            objCarInfo.Reason = "Price should not be empty";

                        }

                        else if (mileage == "")
                        {
                            objCarInfo.AASuccess = "Failure";
                            objCarInfo.Reason = "Mileage should not be empty";

                        }

                        else if (userPackID == "" || packageID == "")
                        {
                            objCarInfo.AASuccess = "Failure";
                            objCarInfo.Reason = "Package should not be empty";
                        }
                    }
                    else
                    {
                        objCarInfo.AASuccess = "Failure";
                        objCarInfo.Reason = "Maxcars count exceeded";
                    }
                }
                else
                {
                    objCarInfo.AASuccess = "Failure";
                    objCarInfo.Reason = "Maxcars count exceeded";
                }


            }
            else
            {
                objCarInfo.AASuccess = "Session timed out";
            }

            obj.Add(objCarInfo);
        }
        catch (Exception ex)
        {
        }
        return obj;
    }


    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
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
            DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("SaveBuyerRequestMobile", CustomerID, AuthenticationID,parameters);
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



    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public string UpdateAllCarFeatures(string CarID, string UID, string FeaturesBulk, string AuthenticationID, string CustomerID, string SessionID)
    {
        string returnStatus = "Failed";
        MobileBL objMobile = new MobileBL();
        DataSet ds = new DataSet();
        try
        {
            if (CustomerID != "")
            {
                string parameters = CarID + "," + UID;
                DataSet dsSaveCustInfo = objMobile.SaveMobileCustomerInfo("UpdateCarFeatures", CustomerID, AuthenticationID, parameters);
            }
            bool bnew = objMobile.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
            if (bnew)
            {
                if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
                {
                    string[] Features = FeaturesBulk.Split(',');
                    for (int i = 0; i < Features.Length; i++)
                    {
                        if (Features[i] != "")
                        {
                            int Feature = Convert.ToInt32(Features[i].Trim());
                            int FeatureID = Feature / 10;
                            int IsActive = Feature % 10;
                            ds = objMobile.mobileUpdateCarfeatures(Convert.ToInt32(CarID), FeatureID, IsActive, Convert.ToInt32(UID));
                            returnStatus = "Success";
                        }
                    }
                }
            }
            else
            {
                returnStatus = "Session timed out";
            }
        }
        catch (Exception ex)
        {
        }
        return returnStatus;
    }

    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public string UploadPictureByCarIDFromAndroid(string CarID, string make, string model, string year, string UserID, string pic, string AuthenticationID, string CustomerID, string SessionID)
    {

        string bStatus = "Failed";
        MobileBL objMobile = new MobileBL();
        UsedCarsInfo objCarPicInfo = new UsedCarsInfo();

        try
        {
            if (CustomerID != "")
            {
                string parameters = CarID + "," + make + "," + model + "," + year + "," + UserID;
                DataSet dsSaveCustInfo = objMobile.SaveMobileCustomerInfo("UploadPictureByCarID", CustomerID, AuthenticationID, parameters);
            }
            bool bnew = objMobile.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UserID));
            if (bnew)
            {
                try
                {
                    if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
                    {
                        objCarPicInfo.Carid = Convert.ToInt32(CarID);
                        byte[] picContent = Convert.FromBase64String(pic);
                        MemoryStream ms = new MemoryStream(picContent);

                        System.Drawing.Bitmap oBitmap1 = new System.Drawing.Bitmap(ms);

                        DataSet dsImagesData = objMobile.GetMobileCarPicIDs(Convert.ToInt32(CarID));

                        string FileNameFullLocation = "CarImages" + "/" + year.ToString() + "/" + make.ToString() + "/" + model + "/";
                        string FileNameFullThumb = "CarImages" + "/" + year.ToString() + "/" + make.ToString() + "/" + model + "/";
                        string FileLocatinon = "MobileService/CarImages" + "/" + year.ToString() + "/" + make.ToString() + "/" + model + "/";
                        string FileName = string.Empty;
                        if (dsImagesData.Tables.Count > 0)
                        {
                            if (dsImagesData.Tables[0].Rows.Count > 0)
                            {
                                objCarPicInfo.PIC0 = dsImagesData.Tables[0].Rows[0]["pic0"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic0"].ToString();
                                objCarPicInfo.PIC1 = dsImagesData.Tables[0].Rows[0]["pic1"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic1"].ToString();
                                objCarPicInfo.PIC2 = dsImagesData.Tables[0].Rows[0]["pic2"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic2"].ToString();
                                objCarPicInfo.PIC3 = dsImagesData.Tables[0].Rows[0]["pic3"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic3"].ToString();
                                objCarPicInfo.PIC4 = dsImagesData.Tables[0].Rows[0]["pic4"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic4"].ToString();
                                objCarPicInfo.PIC5 = dsImagesData.Tables[0].Rows[0]["pic5"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic5"].ToString();
                                objCarPicInfo.PIC6 = dsImagesData.Tables[0].Rows[0]["pic6"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic6"].ToString();
                                objCarPicInfo.PIC7 = dsImagesData.Tables[0].Rows[0]["pic7"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic7"].ToString();
                                objCarPicInfo.PIC8 = dsImagesData.Tables[0].Rows[0]["pic8"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic8"].ToString();
                                objCarPicInfo.PIC9 = dsImagesData.Tables[0].Rows[0]["pic9"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic9"].ToString();
                                objCarPicInfo.PIC10 = dsImagesData.Tables[0].Rows[0]["pic10"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic10"].ToString();
                                objCarPicInfo.PIC11 = dsImagesData.Tables[0].Rows[0]["pic11"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic11"].ToString();
                                objCarPicInfo.PIC12 = dsImagesData.Tables[0].Rows[0]["pic12"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic12"].ToString();
                                objCarPicInfo.PIC13 = dsImagesData.Tables[0].Rows[0]["pic13"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic13"].ToString();
                                objCarPicInfo.PIC14 = dsImagesData.Tables[0].Rows[0]["pic14"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic14"].ToString();
                                objCarPicInfo.PIC15 = dsImagesData.Tables[0].Rows[0]["pic15"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic15"].ToString();
                                objCarPicInfo.PIC16 = dsImagesData.Tables[0].Rows[0]["pic16"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic16"].ToString();
                                objCarPicInfo.PIC17 = dsImagesData.Tables[0].Rows[0]["pic17"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic17"].ToString();
                                objCarPicInfo.PIC18 = dsImagesData.Tables[0].Rows[0]["pic18"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic18"].ToString();
                                objCarPicInfo.PIC19 = dsImagesData.Tables[0].Rows[0]["pic19"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic19"].ToString();
                                objCarPicInfo.PIC20 = dsImagesData.Tables[0].Rows[0]["pic20"].ToString() == "" ? null : dsImagesData.Tables[0].Rows[0]["pic20"].ToString();


                                //******************Creating bigImage***************************

                                if (objCarPicInfo.PIC1 == "0" || objCarPicInfo.PIC1 == null || objCarPicInfo.PIC1 == "" || objCarPicInfo.PIC1 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image1.Jpeg";
                                }
                                else if (objCarPicInfo.PIC2 == "0" || objCarPicInfo.PIC2 == null || objCarPicInfo.PIC2 == "" || objCarPicInfo.PIC2 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image2.Jpeg";
                                }
                                else if (objCarPicInfo.PIC3 == "0" || objCarPicInfo.PIC3 == null || objCarPicInfo.PIC3 == "" || objCarPicInfo.PIC3 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image3.Jpeg";
                                }
                                else if (objCarPicInfo.PIC4 == "0" || objCarPicInfo.PIC4 == null || objCarPicInfo.PIC4 == "" || objCarPicInfo.PIC4 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image4.Jpeg";
                                }
                                else if (objCarPicInfo.PIC5 == "0" || objCarPicInfo.PIC5 == null || objCarPicInfo.PIC5 == "" || objCarPicInfo.PIC5 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image5.Jpeg";
                                }
                                else if (objCarPicInfo.PIC6 == "0" || objCarPicInfo.PIC6 == null || objCarPicInfo.PIC6 == "" || objCarPicInfo.PIC6 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image6.Jpeg";
                                }
                                else if (objCarPicInfo.PIC7 == "0" || objCarPicInfo.PIC7 == null || objCarPicInfo.PIC7 == "" || objCarPicInfo.PIC7 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image7.Jpeg";
                                }
                                else if (objCarPicInfo.PIC8 == "0" || objCarPicInfo.PIC8 == null || objCarPicInfo.PIC8 == "" || objCarPicInfo.PIC8 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image8.Jpeg";
                                }
                                else if (objCarPicInfo.PIC9 == "0" || objCarPicInfo.PIC9 == null || objCarPicInfo.PIC9 == "" || objCarPicInfo.PIC9 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image9.Jpeg";
                                }

                                else if (objCarPicInfo.PIC10 == "0" || objCarPicInfo.PIC10 == null || objCarPicInfo.PIC10 == "" || objCarPicInfo.PIC10 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image10.Jpeg";
                                }
                                else if (objCarPicInfo.PIC11 == "0" || objCarPicInfo.PIC11 == null || objCarPicInfo.PIC11 == "" || objCarPicInfo.PIC11 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image11.Jpeg";
                                }
                                else if (objCarPicInfo.PIC12 == "0" || objCarPicInfo.PIC12 == null || objCarPicInfo.PIC12 == "" || objCarPicInfo.PIC12 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image12.Jpeg";
                                }
                                else if (objCarPicInfo.PIC13 == "0" || objCarPicInfo.PIC13 == null || objCarPicInfo.PIC13 == "" || objCarPicInfo.PIC13 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image13.Jpeg";
                                }
                                else if (objCarPicInfo.PIC14 == "0" || objCarPicInfo.PIC14 == null || objCarPicInfo.PIC14 == "" || objCarPicInfo.PIC14 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image14.Jpeg";
                                }
                                else if (objCarPicInfo.PIC15 == "0" || objCarPicInfo.PIC15 == null || objCarPicInfo.PIC15 == "" || objCarPicInfo.PIC15 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image15.Jpeg";
                                }
                                else if (objCarPicInfo.PIC16 == "0" || objCarPicInfo.PIC16 == null || objCarPicInfo.PIC16 == "" || objCarPicInfo.PIC16 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image16.Jpeg";
                                }
                                else if (objCarPicInfo.PIC17 == "0" || objCarPicInfo.PIC17 == null || objCarPicInfo.PIC17 == "" || objCarPicInfo.PIC17 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image17.Jpeg";
                                }
                                else if (objCarPicInfo.PIC18 == "0" || objCarPicInfo.PIC18 == null || objCarPicInfo.PIC18 == "" || objCarPicInfo.PIC18 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image18.Jpeg";
                                }
                                else if (objCarPicInfo.PIC19 == "0" || objCarPicInfo.PIC19 == null || objCarPicInfo.PIC19 == "" || objCarPicInfo.PIC19 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image19.Jpeg";
                                }
                                else if (objCarPicInfo.PIC20 == "0" || objCarPicInfo.PIC20 == null || objCarPicInfo.PIC20 == "" || objCarPicInfo.PIC20 == " ")
                                {
                                    FileName = year.ToString() + "_" + make + "_" + model + "_" + CarID + "_Image20.Jpeg";
                                }



                                string sFilePath = Server.MapPath(FileNameFullLocation);
                                if (System.IO.Directory.Exists(sFilePath) == false)
                                {
                                    System.IO.Directory.CreateDirectory(sFilePath);
                                }

                                Graphics oGraphic1 = default(Graphics);

                                int newwidthimg1 = 600;
                                // Here create a new bitmap object of the same height and width of the image.
                                float AspectRatio = (float)oBitmap1.Size.Width / (float)oBitmap1.Size.Height;
                                int newHeight1 = Convert.ToInt32(newwidthimg1 / AspectRatio);

                                Bitmap bmpNew1 = new Bitmap(newwidthimg1, newHeight1);
                                oGraphic1 = Graphics.FromImage(bmpNew1);

                                oGraphic1.CompositingQuality = CompositingQuality.HighQuality;
                                oGraphic1.SmoothingMode = SmoothingMode.HighQuality;
                                oGraphic1.InterpolationMode = InterpolationMode.HighQualityBicubic;


                                oGraphic1.DrawImage(oBitmap1, new Rectangle(0, 0, bmpNew1.Width, bmpNew1.Height), 0, 0, oBitmap1.Width, oBitmap1.Height, GraphicsUnit.Pixel);
                                // Release the lock on the image file. Of course,
                                // image from the image file is existing in Graphics object
                                oBitmap1.Dispose();
                                oBitmap1 = bmpNew1;

                                oBitmap1.Save(sFilePath + "/" + FileName, ImageFormat.Jpeg);

                                oBitmap1.Dispose();


                                string picID = objMobile.SaveMobileCarPicture(FileLocatinon, "Jpeg", FileName, Convert.ToInt32(UserID));


                                if (objCarPicInfo.PIC1 == "0" || objCarPicInfo.PIC1 == null || objCarPicInfo.PIC1 == "" || objCarPicInfo.PIC1 == " ")
                                {
                                    objCarPicInfo.PIC1 = picID;
                                }
                                else if (objCarPicInfo.PIC2 == "0" || objCarPicInfo.PIC2 == null || objCarPicInfo.PIC2 == "" || objCarPicInfo.PIC2 == " ")
                                {
                                    objCarPicInfo.PIC2 = picID;
                                }
                                else if (objCarPicInfo.PIC3 == "0" || objCarPicInfo.PIC3 == null || objCarPicInfo.PIC3 == "" || objCarPicInfo.PIC3 == " ")
                                {
                                    objCarPicInfo.PIC3 = picID;
                                }
                                else if (objCarPicInfo.PIC4 == "0" || objCarPicInfo.PIC4 == null || objCarPicInfo.PIC4 == "" || objCarPicInfo.PIC4 == " ")
                                {
                                    objCarPicInfo.PIC4 = picID;
                                }
                                else if (objCarPicInfo.PIC5 == "0" || objCarPicInfo.PIC5 == null || objCarPicInfo.PIC5 == "" || objCarPicInfo.PIC5 == " ")
                                {
                                    objCarPicInfo.PIC5 = picID;
                                }
                                else if (objCarPicInfo.PIC6 == "0" || objCarPicInfo.PIC6 == null || objCarPicInfo.PIC6 == "" || objCarPicInfo.PIC6 == " ")
                                {
                                    objCarPicInfo.PIC6 = picID;
                                }
                                else if (objCarPicInfo.PIC7 == "0" || objCarPicInfo.PIC7 == null || objCarPicInfo.PIC7 == "" || objCarPicInfo.PIC7 == " ")
                                {
                                    objCarPicInfo.PIC7 = picID;
                                }
                                else if (objCarPicInfo.PIC8 == "0" || objCarPicInfo.PIC8 == null || objCarPicInfo.PIC8 == "" || objCarPicInfo.PIC8 == " ")
                                {
                                    objCarPicInfo.PIC8 = picID;
                                }
                                else if (objCarPicInfo.PIC9 == "0" || objCarPicInfo.PIC9 == null || objCarPicInfo.PIC9 == "" || objCarPicInfo.PIC9 == " ")
                                {
                                    objCarPicInfo.PIC9 = picID;
                                }

                                else if (objCarPicInfo.PIC10 == "0" || objCarPicInfo.PIC10 == null || objCarPicInfo.PIC10 == "" || objCarPicInfo.PIC10 == " ")
                                {
                                    objCarPicInfo.PIC10 = picID;
                                }
                                else if (objCarPicInfo.PIC11 == "0" || objCarPicInfo.PIC11 == null || objCarPicInfo.PIC11 == "" || objCarPicInfo.PIC11 == " ")
                                {
                                    objCarPicInfo.PIC11 = picID;
                                }
                                else if (objCarPicInfo.PIC12 == "0" || objCarPicInfo.PIC12 == null || objCarPicInfo.PIC12 == "" || objCarPicInfo.PIC12 == " ")
                                {
                                    objCarPicInfo.PIC12 = picID;
                                }
                                else if (objCarPicInfo.PIC13 == "0" || objCarPicInfo.PIC13 == null || objCarPicInfo.PIC13 == "" || objCarPicInfo.PIC13 == " ")
                                {
                                    objCarPicInfo.PIC13 = picID;
                                }
                                else if (objCarPicInfo.PIC14 == "0" || objCarPicInfo.PIC14 == null || objCarPicInfo.PIC14 == "" || objCarPicInfo.PIC14 == " ")
                                {
                                    objCarPicInfo.PIC14 = picID;
                                }
                                else if (objCarPicInfo.PIC15 == "0" || objCarPicInfo.PIC15 == null || objCarPicInfo.PIC15 == "" || objCarPicInfo.PIC15 == " ")
                                {
                                    objCarPicInfo.PIC15 = picID;
                                }
                                else if (objCarPicInfo.PIC16 == "0" || objCarPicInfo.PIC16 == null || objCarPicInfo.PIC16 == "" || objCarPicInfo.PIC16 == " ")
                                {
                                    objCarPicInfo.PIC16 = picID;
                                }
                                else if (objCarPicInfo.PIC17 == "0" || objCarPicInfo.PIC17 == null || objCarPicInfo.PIC17 == "" || objCarPicInfo.PIC17 == " ")
                                {
                                    objCarPicInfo.PIC17 = picID;
                                }
                                else if (objCarPicInfo.PIC18 == "0" || objCarPicInfo.PIC18 == null || objCarPicInfo.PIC18 == "" || objCarPicInfo.PIC18 == " ")
                                {
                                    objCarPicInfo.PIC18 = picID;
                                }
                                else if (objCarPicInfo.PIC19 == "0" || objCarPicInfo.PIC19 == null || objCarPicInfo.PIC19 == "" || objCarPicInfo.PIC19 == " ")
                                {
                                    objCarPicInfo.PIC19 = picID;
                                }
                                else if (objCarPicInfo.PIC20 == "0" || objCarPicInfo.PIC20 == null || objCarPicInfo.PIC20 == "" || objCarPicInfo.PIC20 == " ")
                                {
                                    objCarPicInfo.PIC20 = picID;
                                }


                                if (objCarPicInfo.PIC0 == "0" || objCarPicInfo.PIC0 == null || objCarPicInfo.PIC0 == "" || objCarPicInfo.PIC0 == " ")
                                {

                                    System.Drawing.Bitmap oBitmap = new System.Drawing.Bitmap(ms);

                                    string sFilePath1 = Server.MapPath(FileNameFullThumb);
                                    string FileNameThumb = year.ToString() + "_" + make + "_" + model + "_" + CarID + "Thumb.Jpeg";
                                    if (System.IO.Directory.Exists(sFilePath1) == false)
                                    {
                                        System.IO.Directory.CreateDirectory(sFilePath1);
                                    }

                                    Graphics oGraphic = default(Graphics);

                                    int newwidthimg = 250;
                                    // Here create a new bitmap object of the same height and width of the image.
                                    float AspectRatio1 = (float)oBitmap.Size.Width / (float)oBitmap.Size.Height;
                                    int newHeight = Convert.ToInt32(newwidthimg / AspectRatio);

                                    Bitmap bmpNew = new Bitmap(newwidthimg, newHeight);
                                    oGraphic = Graphics.FromImage(bmpNew);

                                    oGraphic.CompositingQuality = CompositingQuality.HighQuality;
                                    oGraphic.SmoothingMode = SmoothingMode.HighQuality;
                                    oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;


                                    oGraphic.DrawImage(oBitmap, new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), 0, 0, oBitmap.Width, oBitmap.Height, GraphicsUnit.Pixel);
                                    // Release the lock on the image file. Of course,
                                    // image from the image file is existing in Graphics object
                                    oBitmap.Dispose();
                                    oBitmap = bmpNew;

                                    oBitmap.Save(sFilePath1 + "/" + FileNameThumb, ImageFormat.Jpeg);

                                    oBitmap.Dispose();


                                    string picIDs = objMobile.SaveMobileCarPicture(FileLocatinon, "Jpeg", FileNameThumb, Convert.ToInt32(UserID));
                                    objCarPicInfo.PIC0 = picIDs;
                                }

                                bool bnw = objMobile.UpdateMobilePicturesByCarId(objCarPicInfo);
                                if (bnw)
                                {
                                    bStatus = "Success";
                                }

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
                bStatus = "Session timed out";
            }

        }
        catch (Exception ex)
        {
        }


        return bStatus;
    }


     [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
    [WebMethod(EnableSession = true)]
    public string AddPackageRequestMobile(string UID,string AuthenticationID,string CustomerID, string SessionID)
     {
       string returnStatus = "Failed";
        MobileBL objMobile = new MobileBL();
        DataSet ds = new DataSet();
        try
        {
            if (CustomerID != "")
            {
                string parameters = UID;
                DataSet dsSaveCustInfo = objMobile.SaveMobileCustomerInfo("AddPackageRequestMobile", CustomerID, AuthenticationID, parameters);
            }
            bool bnew = objMobile.CheckMobileAuthorizeUSer(SessionID, Convert.ToInt32(UID));
            if (bnew)
            {
                if (AuthenticationID == ConfigurationManager.AppSettings["AppleID"].ToString())
                {
                    returnStatus = objMobile.SaveAddPackageRequest(UID, CustomerID, AuthenticationID);
                }
               
            }
            else
            {
                returnStatus = "Session timed out";
            }
        }
        catch (Exception ex)
        {
        }
        return returnStatus;
     }


     [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
     [WebMethod(EnableSession = true)]
     public string UpdateSellerInformation(string sellerID, string sellerName, string city, string state, string zip, string phone, string email, string carID, string UID, string AuthenticationID, string CustomerID, string SessionID)
     {
         string returnPostingID = "Failed";

         MobileBL objMobileBL = new MobileBL();
         if (CustomerID.Trim() != "")
         {
             DataSet dsSaveCustInfo = objMobileBL.SaveMobileCustomerInfo("UpdateSellerInformation", CustomerID, AuthenticationID, sellerID);
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


     [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Xml)]
     [WebMethod(EnableSession = true)]
     public List<CarsInfo.UsedCarsInfo> MobileCarsNotificationSearch(string carMake, string CarModal,
                                  string Mileage, string Year, string Price, string Orderby, string Zipcode,string exteriorColor, string interiorColor,
            string Transmission, string VehiceCondition, string DriveTrain, string numberOfCylinder, string numberOfDoors, string fuelType ,string AuthenticationID, string CustomerID)
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
             string parameter = carMake + "," + CarModal + "," + Mileage + "," + Year + "," + Price + "," +Orderby + "," + Zipcode;
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

