using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalientDataConnectivity;
using System.Data;
using System.Configuration;
using HIPAAComponents;
using System.IO;
using System.Globalization;

namespace ProcessHIPAABatches
{
    public partial class _Default : Page
    {
        #region GlobalVariables
        private int intInterchangeID;
        private string strSenderIDQual;
        private string strSenderID;
        private string strReceiverIDQual;
        private string strReceiverID;
        private bool blnAcknowledgement;
        private string strRepetitionSeparator;
        private string strElementSeparator;
        private string strUsage;
        private int intSubmitterEDIContactID;
        private string strContactFunctionCode;
        private string strContactName;
        private string strCommunicationNumberID;
        private string strCommunicationNumber;
        private string strCommunicationNumberID2;
        private string strcommunicationNumber2;
        private string strCommunicationNumberID3;
        private string strcommunicationNumber3;
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            SqlClientDataOperations oSalient = new SqlClientDataOperations();
            DataSet oData = new DataSet();
            DataSet oContact = new DataSet();

            try
            {
                if (!IsPostBack)
                {
                    oSalient.ConnectionString = ConfigurationManager.ConnectionStrings["HIPAAComponents"].ConnectionString;
                    oSalient.LoadDataSetFromStoredProc("usp_ListXChanges").Fill(oData);
                    this.dgInterchanges.DataSource = oData.Tables[0];
                    dgInterchanges.DataBind();
                    oSalient.LoadDataSetFromStoredProc("usp_ListEDIContacts").Fill(oContact);
                    dgEDIContacts.DataSource = oContact.Tables[0];
                    dgEDIContacts.DataBind();
                }
                else
                {
                    intInterchangeID = int.Parse(ViewState["intInterchangeID"].ToString());
                    strSenderIDQual = ViewState["strSenderIDQual"].ToString();
                    strSenderID = ViewState["strSenderID"].ToString();
                    strReceiverID = ViewState["strReceiverID"].ToString();
                    strReceiverIDQual = ViewState["strReceiverIDQaul"].ToString();
                    blnAcknowledgement = bool.Parse(ViewState["blnAcknowledgement"].ToString());
                    strRepetitionSeparator = ViewState["strRepetitionSeparator"].ToString();
                    strElementSeparator = ViewState["strElementSeparator"].ToString();
                    strUsage = ViewState["strUsage"].ToString();
                    intSubmitterEDIContactID = int.Parse(ViewState["SubmitterEDIContactID"].ToString());
                    strContactFunctionCode = ViewState["ContactFunctionCode"].ToString();
                    strContactName = ViewState["ContactName"].ToString();
                    strCommunicationNumberID = ViewState["CommunicationNumberID"].ToString();
                    strCommunicationNumber = ViewState["CommunicationNumber"].ToString();
                    strCommunicationNumberID2 = ViewState["CommunicationNumberID2"].ToString();
                    strcommunicationNumber2 = ViewState["communicationNumber2"].ToString();
                    strCommunicationNumberID3 = ViewState["CommunicationNumberID3"].ToString();
                    strcommunicationNumber3 = ViewState["CommunicationNumber3"].ToString();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        /// <summary>
        /// Build Well-Formed 837P
        /// </summary>
        protected void build837Batches()
        {
            HIPAAComponents.MNBilling oBills = new MNBilling();
            MNBilling.GE_FunctionalGroupTrailer oGE = new MNBilling.GE_FunctionalGroupTrailer();
            MNBilling.IEA_InterchangeControlTrailer oIEA = new MNBilling.IEA_InterchangeControlTrailer();
            SqlClientDataOperations oSalient = new SqlClientDataOperations();
            DataSet oBatchDS = new DataSet();
            DataSet oClaimsDS = new DataSet();
            DataSet oLinesDS = new DataSet();
            //string[] o837P;
            ArrayList o837P = new ArrayList();
            int intSegmentCount;
            int intSVCLineCount;
            int intParent;
            int intHLLevel;
                        
            try
            {                
                oSalient.ConnectionString = ConfigurationManager.ConnectionStrings["HIPAAComponents"].ConnectionString;
                oSalient.LoadDataSetFromStoredProc("usp_GetBatches2Process").Fill(oBatchDS);
                oBills.DefaultSep = "*";
                oBills.EndOfSegment = "~";
                oBills.AckRequested = blnAcknowledgement;

                foreach (DataRow x in oBatchDS.Tables[0].Rows)
                {
                    intSegmentCount = 10;
                    intSVCLineCount = 1;
                    intParent = 1;
                    intHLLevel = 2;
                    o837P.Add(SetISA(x["BatchNumber"].ToString(), oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetGS(x["BatchNumber"].ToString(), oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetST(x["BatchNumber"].ToString(), oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetBHT(x["BatchNumber"].ToString(), oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetSubmitterName(x, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetEDIContact(oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetReceiverName(x, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetBillingProviderLevel(1, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetBillingProviderName(x, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetBillingProviderAddress(x, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetCityStateZip(x, oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetTaxID(x, oBills.DefaultSep, oBills.EndOfSegment));                    

                    ArrayList oParams = new ArrayList();
                    oParams.Add(new System.Data.SqlClient.SqlParameter("@BatchNumber", x["BatchNumber"].ToString()));                    

                    oSalient.LoadDataSet(oParams, "usp_GetClaimsByBatchNumber").Fill(oClaimsDS);
                    foreach(DataRow y in oClaimsDS.Tables[0].Rows)
                    {
                        o837P.Add(SetSubscriberLevel(intHLLevel, intParent, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetSBRSegment(x, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetSubscriberName(y, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetBillingSubscriberAddress(y, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetSubscriberCityStateZip(y, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetSubscriberDemographics(y, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetPayerName(y, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetClaimSegment(y, oBills.DefaultSep, oBills.EndOfSegment));
                        o837P.Add(SetHISegment(y, oBills.DefaultSep, oBills.EndOfSegment));

                        ArrayList oSVCParams = new ArrayList();
                        oSVCParams.Add(new System.Data.SqlClient.SqlParameter("@BatchNumber", x["BatchNumber"].ToString()));
                        oSVCParams.Add(new System.Data.SqlClient.SqlParameter("@SubscriberID", y["ClaimsBatchID"].ToString()));

                        oSalient.LoadDataSet(oSVCParams, "usp_GetSVCLinesByBatchAndClaim").Fill(oLinesDS);
                        foreach(DataRow q in oLinesDS.Tables[0].Rows)
                        { 
                            o837P.Add(SetLXSegment(intSVCLineCount, oBills.DefaultSep, oBills.EndOfSegment));
                            intSVCLineCount += 1;
                            o837P.Add(SetSV1Segment(q, oBills.DefaultSep, oBills.EndOfSegment));
                            o837P.Add(SetServiceDate(q, oBills.DefaultSep, oBills.EndOfSegment));
                            intSegmentCount = intSegmentCount + 3;
                        }
                        intSegmentCount = intSegmentCount + 9;
                        intHLLevel += 1;
                        oLinesDS.Clear();
                        intSVCLineCount = 1;
                    }
                    intSegmentCount = intSegmentCount + 1;
                    o837P.Add(SetSESegment(intSegmentCount, x["BatchNumber"].ToString(), oBills.DefaultSep, oBills.EndOfSegment));
                    o837P.Add(SetGESegment(oBills.DefaultSep, oBills.EndOfSegment, x["BatchNumber"].ToString()));
                    o837P.Add(SetIEASegment(oBills.DefaultSep, oBills.EndOfSegment, x["BatchNumber"].ToString()));

                    int intBuffer = o837P[0].ToString().Length * (intSegmentCount + 6);

                    StreamWriter oFS = new StreamWriter(ConfigurationManager.AppSettings["ResultsPath"].ToString() +  x["NPI"].ToString() + "_837P_" + System.DateTime.Now.ToString("yyyyMMdd") + ".dat");
                    
                    foreach(string z in o837P)
                    {
                        oFS.Write(z);
                        oFS.Flush();
                        ArrayList oInsertParam = new ArrayList();
                        oInsertParam.Add(new System.Data.SqlClient.SqlParameter("@ControlNumber", x["BatchNumber"].ToString()));
                        oInsertParam.Add(new System.Data.SqlClient.SqlParameter("@LineSegment", z.ToString()));
                        //oSalient.ExecuteNonQuery(oInsertParam, "usp_InsertClaimLine");
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetIEASegment(string strDefaultSep, string strEOS, string strControlNo)
        {
            MNBilling.IEA_InterchangeControlTrailer IEA = new MNBilling.IEA_InterchangeControlTrailer();

            try
            {
                IEA.FunctionalGroupCount = 1;
                IEA.ControlNo = strControlNo;

                return "IEA" + strDefaultSep + "1" + strDefaultSep + strControlNo + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetGESegment(string strDefaultSep,string strEOS, string strControlNo)
        {
            MNBilling.GE_FunctionalGroupTrailer GE = new MNBilling.GE_FunctionalGroupTrailer();

            try
            {
                return "GE" + strDefaultSep + "1" + strDefaultSep + strControlNo + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected  string SetSESegment(int intSVCLineCount,string strControlNo,string strDefaultSep,string strEOS)
        {
            SE_TransactionSetTrailer SE = new SE_TransactionSetTrailer();

            try
            {
                SE.SE01__TransactionSegmentCount = intSVCLineCount.ToString();
                SE.SE02__TransactionSetControlNumber = strControlNo;

                return "SE" + strDefaultSep + SE.SE01__TransactionSegmentCount + strDefaultSep + SE.SE02__TransactionSetControlNumber + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetServiceDate(DataRow x,string strDefaultSep,string strEOS)
        {
            DTP_DateServiceDate_2400 DTP = new DTP_DateServiceDate_2400();

            try
            {
                DTP.DTP01__DateTimeQualifier = DTP_DateServiceDate_2400DTP01__DateTimeQualifier.Item472;
                DTP.DTP02__DateTimePeriodFormatQualifier = DTP_DateServiceDate_2400DTP02__DateTimePeriodFormatQualifier.D8;
                DTP.DTP03__ServiceDate = x["DateOfService"].ToString();

                return "DTP" + strDefaultSep + "472" + strDefaultSep + "D8" + strDefaultSep + DTP.DTP03__ServiceDate + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSV1Segment(DataRow x,string strDefaultSep,string strEOS)
        {
            SV1_ProfessionalService_2400 SV = new SV1_ProfessionalService_2400();

            try
            {
                //SV.SV101_CompositeMedicalProcedureIdentifier_2400.SV101_01_ProductOrServiceIDQualifier = SV1_ProfessionalService_2400SV101_CompositeMedicalProcedureIdentifier_2400SV101_01_ProductOrServiceIDQualifier.HC;
                //SV.SV101_CompositeMedicalProcedureIdentifier_2400.SV101_02_ProcedureCode = x["ProcedureCode"].ToString();
                //SV.SV101_CompositeMedicalProcedureIdentifier_2400.SV101_03_ProcedureModifier = x["Modifiers"].ToString();
                SV.SV102__LineItemChargeAmount = x["RatePerUnit"].ToString().Replace(".0000",".00");
                SV.SV103__UnitOrBasisForMeasurementCode = SV1_ProfessionalService_2400SV103__UnitOrBasisForMeasurementCode.UN;
                SV.SV104__ServiceUnitCount = x["Units"].ToString().Replace(".0000","");
                SV.SV105__PlaceOfServiceCode = x["POS"].ToString();
                SV.SV106 = "";
                //SV.SV107_CompositeDiagnosisCodePointer_2400.SV107_01_DiagnosisCodePointer = "1";
                SV.SV108 = "";
                //SV.SV109__EmergencyIndicator = SV1_ProfessionalService_2400SV109__EmergencyIndicator.Y;
                SV.SV110 = "";

                return "SV1" + strDefaultSep + "HC" + strElementSeparator + x["ProcedureCode"].ToString() + strElementSeparator +
                        x["Modifiers"].ToString() + strDefaultSep + SV.SV102__LineItemChargeAmount + strDefaultSep +
                        "UN" + strDefaultSep + SV.SV104__ServiceUnitCount + strDefaultSep + "14" + strDefaultSep + SV.SV106 + strDefaultSep + "1" +
                        strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetLXSegment(int intLineCount,string strDefaultSep,string strEOS)
        {
            LX_ServiceLineNumber_2400 LX = new LX_ServiceLineNumber_2400();

            try
            {
                LX.LX01__AssignedNumber = intLineCount.ToString();

                return "LX" + strDefaultSep + LX.LX01__AssignedNumber + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetHISegment(DataRow x,string strDefaultSep,string strEOS)
        {
            HI_HealthCareDiagnosisCode_2300 HI = new HI_HealthCareDiagnosisCode_2300();
            

            try
            {
                //HI.HI01_HealthCareCodeInformation_2300.HI01_01_DiagnosisTypeCode = HI_HealthCareDiagnosisCode_2300HI01_HealthCareCodeInformation_2300HI01_01_DiagnosisTypeCode.ABK;
                //HI.HI01_HealthCareCodeInformation_2300.HI01_02_DiagnosisCode = x["DiagnosisCode"].ToString();

                return "HI" + strDefaultSep + "ABK" + strElementSeparator + x["DiagnosisCode"].ToString() + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetClaimSegment(DataRow x, string strDefaultSep, string strEOS)
        {
            CLM_ClaimInformation_2300 CLM = new CLM_ClaimInformation_2300();
            CLM_ClaimInformation_2300CLM05_HealthCareServiceLocationInformation_2300 CLM05 = new CLM_ClaimInformation_2300CLM05_HealthCareServiceLocationInformation_2300();

            try
            {
                CLM.CLM01__PatientControlNumber = x["ClaimsBatchID"].ToString();
                CLM.CLM02__TotalClaimChargeAmount = string.Format("{0:0.00}",x["RatePerUnit"]);
                CLM.CLM03 = "";
                CLM.CLM04 = "";
                CLM05.CLM05_01_PlaceOfServiceCode = x["POS"].ToString();
                CLM05.CLM05_02_FacilityCodeQualifier = CLM_ClaimInformation_2300CLM05_HealthCareServiceLocationInformation_2300CLM05_02_FacilityCodeQualifier.B;
                CLM05.CLM05_03_ClaimFrequencyCode = "1";
                CLM.CLM05_HealthCareServiceLocationInformation_2300 = CLM05;
                CLM.CLM06__ProviderOrSupplierSignatureIndicator = CLM_ClaimInformation_2300CLM06__ProviderOrSupplierSignatureIndicator.N;
                CLM.CLM07__AssignmentOrPlanParticipationCode = CLM_ClaimInformation_2300CLM07__AssignmentOrPlanParticipationCode.A;
                CLM.CLM08__BenefitsAssignmentCertificationIndicator = CLM_ClaimInformation_2300CLM08__BenefitsAssignmentCertificationIndicator.Y;
                CLM.CLM09__ReleaseOfInformationCode = CLM_ClaimInformation_2300CLM09__ReleaseOfInformationCode.Y;

                return "CLM" + strDefaultSep + CLM.CLM01__PatientControlNumber + strDefaultSep + CLM.CLM02__TotalClaimChargeAmount + strDefaultSep + CLM.CLM03 +
                        strDefaultSep + CLM.CLM04 + strDefaultSep + CLM05.CLM05_01_PlaceOfServiceCode + strElementSeparator + "B" + strElementSeparator + CLM05.CLM05_03_ClaimFrequencyCode +
                        strDefaultSep + "N" + strDefaultSep + "A" + strDefaultSep + "Y" + strDefaultSep + "Y" + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetPayerName(DataRow x, string strDefaultSep, string strEOS)
        {
            NM1_PayerName_2010BB Q = new NM1_PayerName_2010BB();

            try
            {
                Q.NM101__EntityIdentifierCode = NM1_PayerName_2010BBNM101__EntityIdentifierCode.PR;
                Q.NM102__EntityTypeQualifier = NM1_PayerName_2010BBNM102__EntityTypeQualifier.Item2;
                Q.NM103__PayerName = x["Payer"].ToString();
                Q.NM104 = "";
                Q.NM105 = "";
                Q.NM106 = "";
                Q.NM107 = "";
                Q.NM108__IdentificationCodeQualifier = NM1_PayerName_2010BBNM108__IdentificationCodeQualifier.PI;
                Q.NM109__PayerIdentifier = x["PayerID"].ToString();

                return "NM1" + strDefaultSep + "PR" + strDefaultSep + "2" + strDefaultSep + Q.NM103__PayerName +
                        strDefaultSep + Q.NM104 + strDefaultSep + Q.NM105 + strDefaultSep +
                        Q.NM106 + strDefaultSep + Q.NM107 + strDefaultSep + "PI" + strDefaultSep + Q.NM109__PayerIdentifier + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetEDIContact(string strDefaultSep,string strEOS)
        {
            PER_SubmitterEDIContactInformation_1000A P = new PER_SubmitterEDIContactInformation_1000A();
            string strReturn;

            try
            {
                P.PER01__ContactFunctionCode = PER_SubmitterEDIContactInformation_1000APER01__ContactFunctionCode.IC;
                P.PER02__SubmitterContactName = strContactName;
                switch (strCommunicationNumberID)
                {
                    case "EM":
                        P.PER03__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER03__CommunicationNumberQualifier.EM;
                        break;
                    case "FX":
                        P.PER03__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER03__CommunicationNumberQualifier.FX;
                        break;
                    case "TE":
                        P.PER03__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER03__CommunicationNumberQualifier.TE;
                        break;
                    default:
                        break;
                }                
                P.PER04__CommunicationNumber = strCommunicationNumber;
                switch (strCommunicationNumberID2)
                {
                    case "EM":
                        P.PER05__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER05__CommunicationNumberQualifier.EM;
                        break;
                    case "EX":
                        P.PER05__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER05__CommunicationNumberQualifier.EX;
                        break;
                    case "FX":
                        P.PER05__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER05__CommunicationNumberQualifier.FX;
                        break;
                    case "TE":
                        P.PER05__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER05__CommunicationNumberQualifier.TE;
                        break;
                    default:
                        break;
                }
                P.PER06__CommunicationNumber = strcommunicationNumber2;
                switch (strCommunicationNumberID2)
                {
                    case "EM":
                        P.PER07__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER07__CommunicationNumberQualifier.EM;
                        break;
                    case "EX":
                        P.PER07__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER07__CommunicationNumberQualifier.EX;
                        break;
                    case "FX":
                        P.PER07__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER07__CommunicationNumberQualifier.FX;
                        break;
                    case "TE":
                        P.PER07__CommunicationNumberQualifier = PER_SubmitterEDIContactInformation_1000APER07__CommunicationNumberQualifier.TE;
                        break;
                    default:
                        break;
                }
                P.PER08__CommunicationNumber = strcommunicationNumber3;

                strReturn = "PER" + strDefaultSep + "IC" + strDefaultSep + P.PER02__SubmitterContactName + strDefaultSep + P.PER03__CommunicationNumberQualifier + strDefaultSep + P.PER04__CommunicationNumber;
                if(strCommunicationNumberID2.Length == 2)
                {
                    strReturn = strReturn + strDefaultSep + P.PER05__CommunicationNumberQualifier + strDefaultSep + P.PER06__CommunicationNumber;
                    if (strCommunicationNumberID3.Length == 2)
                    {
                        strReturn = strReturn + strDefaultSep + P.PER07__CommunicationNumberQualifier + strDefaultSep + P.PER08__CommunicationNumber;
                    }
                }
                
                strReturn = strReturn + strEOS;

                return strReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSubscriberName(DataRow x, string strDefaultSep, string strEOS)
        {
            NM1_SubscriberName_2010BA Q = new NM1_SubscriberName_2010BA();

            try
            {
                Q.NM101__EntityIdentifierCode = NM1_SubscriberName_2010BANM101__EntityIdentifierCode.IL;
                Q.NM102__EntityTypeQualifier = NM1_SubscriberName_2010BANM102__EntityTypeQualifier.Item1;
                Q.NM103__SubscriberLastName = x["SubscriberLast"].ToString();
                Q.NM104__SubscriberFirstName = x["SubscriberFirst"].ToString();
                Q.NM105__SubscriberMiddleNameOrInitial = x["SubscriberMiddle"].ToString();
                Q.NM106 = "";
                Q.NM107__SubscriberNameSuffix = "";
                Q.NM108__IdentificationCodeQualifier = NM1_SubscriberName_2010BANM108__IdentificationCodeQualifier.MI;
                Q.NM109__SubscriberPrimaryIdentifier = x["SubscriberID"].ToString();

                return "NM1" + strDefaultSep + "IL" + strDefaultSep + "1" + strDefaultSep + Q.NM103__SubscriberLastName +
                        strDefaultSep + Q.NM104__SubscriberFirstName + strDefaultSep + Q.NM105__SubscriberMiddleNameOrInitial + strDefaultSep +
                        Q.NM106 + strDefaultSep + Q.NM107__SubscriberNameSuffix + strDefaultSep + "MI" + strDefaultSep + Q.NM109__SubscriberPrimaryIdentifier + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSBRSegment(DataRow x,string strDefault,string strEOS)
        {
            SBR_SubscriberInformation_2000B SUB = new SBR_SubscriberInformation_2000B();

            try
            {
                SUB.SBR01__PayerResponsibilitySequenceNumberCode = SBR_SubscriberInformation_2000BSBR01__PayerResponsibilitySequenceNumberCode.U;
                SUB.SBR02__IndividualRelationshipCode = SBR_SubscriberInformation_2000BSBR02__IndividualRelationshipCode.Item18;
                SUB.SBR03__SubscriberGroupOrPolicyNumber = "";
                SUB.SBR04__SubscriberGroupName = "";
                SUB.SBR05__InsuranceTypeCode = SBR_SubscriberInformation_2000BSBR05__InsuranceTypeCode.Item12;
                SUB.SBR06 = "";
                SUB.SBR07 = "";
                SUB.SBR08 = "";
                SUB.SBR09__ClaimFilingIndicatorCode = SBR_SubscriberInformation_2000BSBR09__ClaimFilingIndicatorCode.MC;

                return "SBR" + strDefault + "U" + strDefault + "18" + strDefault + SUB.SBR03__SubscriberGroupOrPolicyNumber + strDefault +
                        SUB.SBR04__SubscriberGroupName + strDefault + "" + strDefault + SUB.SBR06 + strDefault + SUB.SBR07 + strDefault + SUB.SBR08 + strDefault +
                        "MC" + strEOS;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetTaxID(DataRow x,string strDefaultSep,string strEOS)
        {
            REF_BillingProviderTaxIdentification_2010AA TID = new REF_BillingProviderTaxIdentification_2010AA();

            try
            {
                TID.REF01__ReferenceIdentificationQualifier = REF_BillingProviderTaxIdentification_2010AAREF01__ReferenceIdentificationQualifier.EI;
                TID.REF02__BillingProviderTaxIdentificationNumber = x["StateID"].ToString();

                return "REF" + strDefaultSep + TID.REF01__ReferenceIdentificationQualifier + strDefaultSep + TID.REF02__BillingProviderTaxIdentificationNumber + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSubscriberLevel(int intHID,int intParentID,string strDEfaultSep,string strEOS)
        {
            HL_SubscriberHierarchicalLevel_2000B HL = new HL_SubscriberHierarchicalLevel_2000B();

            try
            {
                HL.HL01__HierarchicalIDNumber = intHID.ToString();
                HL.HL02__HierarchicalParentIDNumber = intParentID.ToString();
                HL.HL03__HierarchicalLevelCode = HL_SubscriberHierarchicalLevel_2000BHL03__HierarchicalLevelCode.Item22;
                HL.HL04__HierarchicalChildCode = HL_SubscriberHierarchicalLevel_2000BHL04__HierarchicalChildCode.Item0;

                return "HL" + strDEfaultSep + HL.HL01__HierarchicalIDNumber + strDEfaultSep + HL.HL02__HierarchicalParentIDNumber +
                        strDEfaultSep + "22" + strDEfaultSep + "0" + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetCityStateZip(DataRow x,string strDefault, string strEOS)
        {
            N4_BillingProviderCityStateZIPCode_2010AA BC = new N4_BillingProviderCityStateZIPCode_2010AA();

            try
            {
                BC.N401__BillingProviderCityName = "St. Paul";
                BC.N402__BillingProviderStateOrProvinceCode = "MN";
                BC.N403__BillingProviderPostalZoneOrZIPCode = "55107";

                return "N4" + strDefault + BC.N401__BillingProviderCityName + strDefault + BC.N402__BillingProviderStateOrProvinceCode + strDefault + BC.N403__BillingProviderPostalZoneOrZIPCode + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSubscriberDemographics(DataRow x, string strDefaultSep,string strEOS)
        {
            DMG_SubscriberDemographicInformation_2010BA d = new DMG_SubscriberDemographicInformation_2010BA();

            try
            {
                d.DMG01__DateTimePeriodFormatQualifier = DMG_SubscriberDemographicInformation_2010BADMG01__DateTimePeriodFormatQualifier.D8;
                d.DMG02__SubscriberBirthDate = x["SubscriberDOB"].ToString();
                switch (x["SubscriberGender"].ToString())
                {
                    case "M":
                        d.DMG03__SubscriberGenderCode = DMG_SubscriberDemographicInformation_2010BADMG03__SubscriberGenderCode.M;
                        break;
                    case "F":
                        d.DMG03__SubscriberGenderCode = DMG_SubscriberDemographicInformation_2010BADMG03__SubscriberGenderCode.F;
                        break;
                    default:
                        d.DMG03__SubscriberGenderCode = DMG_SubscriberDemographicInformation_2010BADMG03__SubscriberGenderCode.U;
                        break;
                }

                return "DMG" + strDefaultSep + "D8" + strDefaultSep + d.DMG02__SubscriberBirthDate + strDefaultSep + x["SubscriberGender"].ToString() + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSubscriberCityStateZip(DataRow x, string strDefault, string strEOS)
        {
            N4_SubscriberCityStateZIPCode_2010BA BC = new N4_SubscriberCityStateZIPCode_2010BA();

            try
            {
                BC.N401__SubscriberCityName = x["SubscriberCity"].ToString();
                BC.N402__SubscriberStateCode = x["SubscriberState"].ToString();
                BC.N403__SubscriberPostalZoneOrZIPCode = x["SubscriberZip"].ToString();

                return "N4" + strDefault + BC.N401__SubscriberCityName + strDefault + BC.N402__SubscriberStateCode + strDefault + BC.N403__SubscriberPostalZoneOrZIPCode + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetBillingSubscriberAddress(DataRow x, string strDefaultSep, string strEOS)
        {
            N3_SubscriberAddress_2010BA A = new N3_SubscriberAddress_2010BA();
            string strResults;

            try
            {
                A.N301__SubscriberAddressLine = x["SubscriberAddress1"].ToString();
                A.N302__SubscriberAddressLine = x["SubscriberAddress2"].ToString();

                strResults = "N3" + strDefaultSep + A.N301__SubscriberAddressLine;
                if (A.N302__SubscriberAddressLine.Length > 0)
                {
                    strResults = strResults + strDefaultSep + A.N302__SubscriberAddressLine;
                }
                strResults = strResults + strEOS;

                return strResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetBillingProviderAddress(DataRow x, string strDefaultSep,string strEOS)
        {
            N3_BillingProviderAddress_2010AA A = new N3_BillingProviderAddress_2010AA();

            try
            {
                A.N301__BillingProviderAddressLine = "30 East Plato Boulevard";

                return "N3" + strDefaultSep + A.N301__BillingProviderAddressLine + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetBillingProviderName(DataRow x,string strDefaultSep,string strEOS)
        {
            MNBilling oNM1 = new MNBilling();
            NM1_BillingProviderName_2010AA Q = new NM1_BillingProviderName_2010AA();

            try
            {
                Q.NM101__EntityIdentifierCode = NM1_BillingProviderName_2010AANM101__EntityIdentifierCode.Item85;
                Q.NM102__EntityTypeQualifier = NM1_BillingProviderName_2010AANM102__EntityTypeQualifier.Item2;
                Q.NM103__BillingProviderLastOrOrganizationalName = x["Biller"].ToString();
                Q.NM104__BillingProviderFirstName = "";
                Q.NM105__BillingProviderMiddleNameOrInitial = "";
                Q.NM106 = "";
                Q.NM107__BillingProviderNameSuffix = "";
                Q.NM108__IdentificationCodeQualifier = NM1_BillingProviderName_2010AANM108__IdentificationCodeQualifier.XX;
                Q.NM109__BillingProviderIdentifier = x["NPI"].ToString();

                return "NM1" + strDefaultSep + "85" + strDefaultSep + "2" + strDefaultSep + Q.NM103__BillingProviderLastOrOrganizationalName +
                        strDefaultSep + Q.NM104__BillingProviderFirstName + strDefaultSep + Q.NM105__BillingProviderMiddleNameOrInitial + strDefaultSep +
                        Q.NM106 + strDefaultSep + Q.NM107__BillingProviderNameSuffix + strDefaultSep + "XX" + strDefaultSep + Q.NM109__BillingProviderIdentifier + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetBillingProviderLevel(int intHID, string strDefaultSep, string strEOS)
        {
            MNBilling oHL = new MNBilling();
            HL_BillingProviderHierarchicalLevel_2000A Y = new HL_BillingProviderHierarchicalLevel_2000A();

            try
            {                
                Y.HL01__HierarchicalIDNumber = "1";
                Y.HL02 = "";
                Y.HL03__HierarchicalLevelCode = HL_BillingProviderHierarchicalLevel_2000AHL03__HierarchicalLevelCode.Item20;
                Y.HL04__HierarchicalChildCode = HL_BillingProviderHierarchicalLevel_2000AHL04__HierarchicalChildCode.Item1;

                return "HL" + strDefaultSep + Y.HL01__HierarchicalIDNumber + strDefaultSep + Y.HL02 + strDefaultSep + "20" + strDefaultSep + "1" + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetReceiverName(DataRow x, string strDefaultSep, string strEOS)
        {
            MNBilling oNM1 = new MNBilling();
            NM1_ReceiverName_1000B Q = new NM1_ReceiverName_1000B();

            try
            {
               
                Q.NM101__EntityIdentifierCode = NM1_ReceiverName_1000BNM101__EntityIdentifierCode.Item40;
                Q.NM102__EntityTypeQualifier = NM1_ReceiverName_1000BNM102__EntityTypeQualifier.Item2;
                Q.NM103__ReceiverName = x["Payer"].ToString();
                Q.NM104 = "";
                Q.NM105 = "";
                Q.NM106 = "";
                Q.NM107 = "";
                Q.NM108__IdentificationCodeQualifier = NM1_ReceiverName_1000BNM108__IdentificationCodeQualifier.Item46;
                Q.NM109__ReceiverPrimaryIdentifier = x["PayerID"].ToString();

                return "NM1" + strDefaultSep + "40" + strDefaultSep + "2" + strDefaultSep + Q.NM103__ReceiverName +
                        strDefaultSep + Q.NM104 + strDefaultSep + Q.NM105 + strDefaultSep +
                        Q.NM106 + strDefaultSep + Q.NM107 + strDefaultSep + "46" + strDefaultSep + Q.NM109__ReceiverPrimaryIdentifier + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetSubmitterName(DataRow x, string strDefaultSep, string strEOS)
        {
            MNBilling oNM1 = new MNBilling();
            NM1_SubmitterName_1000A Q = new NM1_SubmitterName_1000A();

            try
            {
                
                Q.NM101__EntityIdentifierCode = NM1_SubmitterName_1000ANM101__EntityIdentifierCode.Item41;
                Q.NM102__EntityTypeQualifier = NM1_SubmitterName_1000ANM102__EntityTypeQualifier.Item2;
                Q.NM103__SubmitterLastOrOrganizationName = x["Biller"].ToString();
                Q.NM104__SubmitterFirstName = "";
                Q.NM105__SubmitterMiddleNameOrInitial = "";
                Q.NM106 = "";
                Q.NM107 = "";
                Q.NM108__IdentificationCodeQualifier = NM1_SubmitterName_1000ANM108__IdentificationCodeQualifier.Item46;
                Q.NM109__SubmitterIdentifier = x["NPI"].ToString();

                return "NM1" + strDefaultSep + "41" + strDefaultSep + "2" + strDefaultSep + Q.NM103__SubmitterLastOrOrganizationName +
                        strDefaultSep + Q.NM104__SubmitterFirstName + strDefaultSep + Q.NM105__SubmitterMiddleNameOrInitial + strDefaultSep +
                        Q.NM106 + strDefaultSep + Q.NM107 + strDefaultSep + "46" + strDefaultSep + Q.NM109__SubmitterIdentifier + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetBHT(string strControlNo,string strDefaultSep,string strEOS)
        {
            MNBilling oBHT = new MNBilling();
            BHT_BeginningOfHierarchicalTransaction P = new BHT_BeginningOfHierarchicalTransaction();

            try
            {              
                
                P.BHT01__HierarchicalStructureCode = BHT_BeginningOfHierarchicalTransactionBHT01__HierarchicalStructureCode.Item0019;
                P.BHT02__TransactionSetPurposeCode = BHT_BeginningOfHierarchicalTransactionBHT02__TransactionSetPurposeCode.Item00;
                P.BHT03__OriginatorApplicationTransactionIdentifier = System.DateTime.Now.ToString("yyyyMMddHHmmss");
                P.BHT04__TransactionSetCreationDate = System.DateTime.Now.ToString("yyyyMMdd");
                P.BHT05__TransactionSetCreationTime = System.DateTime.Now.ToString("HHmm");
                P.BHT06__ClaimOrEncounterIdentifier = BHT_BeginningOfHierarchicalTransactionBHT06__ClaimOrEncounterIdentifier.CH;

                return "BHT" + strDefaultSep + "0019" + strDefaultSep + "00" + strDefaultSep + P.BHT03__OriginatorApplicationTransactionIdentifier +
                    strDefaultSep + P.BHT04__TransactionSetCreationDate + strDefaultSep + P.BHT05__TransactionSetCreationTime + strDefaultSep + "CH" + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetST(string strControlNo, string strDefaultSep, string strEOS)
        {
            MNBilling oST = new MNBilling();

            try
            {
                oST.ST_TransactionSetHeader = new ST_TransactionSetHeader();
                oST.ST_TransactionSetHeader.ST02__TransactionSetControlNumber = strControlNo;
                oST.ST_TransactionSetHeader.ST01__TransactionSetIdentifierCode = ST_TransactionSetHeaderST01__TransactionSetIdentifierCode.Item837;
                oST.ST_TransactionSetHeader.ST03__ImplementationGuideVersionName = "005010X222A1";

                return "ST" + strDefaultSep + "837" + strDefaultSep + oST.ST_TransactionSetHeader.ST02__TransactionSetControlNumber + strDefaultSep +  oST.ST_TransactionSetHeader.ST03__ImplementationGuideVersionName + strEOS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string SetGS(string strControlNo,string strDefaultSep, string strEOS)
        {
            MNBilling.GS_FunctionalGroupHeader oGS = new MNBilling.GS_FunctionalGroupHeader();

            try
            {
                oGS.ControlNo = strControlNo;
                oGS.SetDefaultSep = strDefaultSep;
                oGS.SetEOS = strEOS;
                oGS.GSID = oGS.GetIDCodeFromString("HC");
                oGS.ReceiverID = strReceiverID;
                oGS.SenderID = strSenderID;

                return oGS.GS_Segment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Setting Up the ISA Segment
        /// </summary>
        protected string SetISA(string strControlNo,string strDefaultSep, string strEOS)
        {
            MNBilling.ISA_InterchangeControlHeader oISA = new MNBilling.ISA_InterchangeControlHeader();
            try
            {
                oISA.ControlNo = strControlNo;
                oISA.Acknowledgement(blnAcknowledgement);
                oISA.ReceiverID = strReceiverID;
                oISA.ReceiverQual = oISA.ReturnXQualValueFromString(strReceiverIDQual);
                oISA.SenderQual = oISA.ReturnXQualValueFromString(strSenderIDQual);
                oISA.SenderID = strSenderID;
                oISA.EleSep = strElementSeparator;
                oISA.RepSep = strRepetitionSeparator;
                oISA.Usage = strUsage;
                oISA.SetDefaultSep = strDefaultSep;
                oISA.SetEOS = strEOS;

                return oISA.ISASegment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnBuildBatches_Click(object sender, EventArgs e)
        {
            try
            {
                build837Batches();
            }
            catch(Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        protected void dgInterchanges_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                intInterchangeID = int.Parse(dgInterchanges.SelectedItem.Cells[0].Text);
                strSenderIDQual = dgInterchanges.SelectedItem.Cells[1].Text;
                strSenderID = dgInterchanges.SelectedItem.Cells[2].Text;
                strReceiverIDQual = dgInterchanges.SelectedItem.Cells[3].Text;
                strReceiverID = dgInterchanges.SelectedItem.Cells[4].Text;
                blnAcknowledgement = bool.Parse(dgInterchanges.SelectedItem.Cells[5].Text);
                strRepetitionSeparator = dgInterchanges.SelectedItem.Cells[6].Text;
                strElementSeparator = dgInterchanges.SelectedItem.Cells[7].Text;
                strUsage = dgInterchanges.SelectedItem.Cells[8].Text;
                ViewState["intInterchangeID"] = intInterchangeID;
                ViewState["strSenderIDQual"] = strSenderIDQual;
                ViewState["strSenderID"] = strSenderID;
                ViewState["strReceiverID"] = strReceiverID;
                ViewState["strReceiverIDQaul"] = strReceiverIDQual;
                ViewState["blnAcknowledgement"] = blnAcknowledgement;
                ViewState["strRepetitionSeparator"] = strRepetitionSeparator;
                ViewState["strElementSeparator"] = strElementSeparator;
                ViewState["strUsage"] = strUsage;
            }
            catch(Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        protected void dgEDIContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                intSubmitterEDIContactID = int.Parse(dgEDIContacts.SelectedItem.Cells[0].Text);
                strContactFunctionCode = dgEDIContacts.SelectedItem.Cells[1].Text;
                strContactName = dgEDIContacts.SelectedItem.Cells[2].Text;
                strCommunicationNumberID = dgEDIContacts.SelectedItem.Cells[3].Text;
                strCommunicationNumber = dgEDIContacts.SelectedItem.Cells[4].Text;
                strCommunicationNumberID2 = dgEDIContacts.SelectedItem.Cells[5].Text;
                strcommunicationNumber2 = dgEDIContacts.SelectedItem.Cells[6].Text;
                strCommunicationNumberID3 = dgEDIContacts.SelectedItem.Cells[7].Text;
                strcommunicationNumber3 = dgEDIContacts.SelectedItem.Cells[8].Text;
                ViewState["SubmitterEDIContactID"] = intSubmitterEDIContactID;
                ViewState["ContactFunctionCode"] = strContactFunctionCode;
                ViewState["ContactName"] = strContactName;
                ViewState["CommunicationNumberID"] = strCommunicationNumberID;
                ViewState["CommunicationNumber"] = strCommunicationNumber;
                ViewState["CommunicationNumberID2"] = strCommunicationNumberID2;
                ViewState["communicationNumber2"] = strcommunicationNumber2;
                ViewState["CommunicationNumberID3"] = strCommunicationNumberID3;
                ViewState["CommunicationNumber3"] = strcommunicationNumber3;
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}