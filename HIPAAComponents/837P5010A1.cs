using System;

namespace HIPAAComponents
{
    public class MNBilling:X12_005010X222A2_837Q1
    {
        private string strElementSeparator = ":";
        private string strRepetitionSeparator = "^";
        private bool boolAckRequested = true;
        private string strUsage = "P";
        private string strDefaultSep = "*";
        private string strEOS = "~";

        public string DefaultSep
        {
            get
            {
                return strDefaultSep;
            }
            set
            {
                strDefaultSep = value;
            }
        }

        public string EndOfSegment
        {
            get
            {
                return strEOS;
            }
            set
            {
                strEOS = value;
            }
        }

        public string ElementSeparator
        {
            set
            {
                strElementSeparator = value;
            }
        }

        public string RepetitionSeparator
        {
            set
            {
                strRepetitionSeparator = value;
            }
        }

        public string Usage
        {
            set
            {
                strUsage = value;
            }
        }

        public bool AckRequested
        {
            set
            {
                boolAckRequested = value;
            }
        }

        //public partial class ClaimsEnvelope
        //{
        //    private ISA_InterchangeControlHeader ISAHeader;
        //    private X12_005010X222A2_837Q1 ClaimsBody;

        //}

        public partial class ISA_InterchangeControlHeader
        {
            private string strISA01_AuthQual = "00";
            private const string str10Spaces = "          ";
            private const string str15Spaces = "               ";
            private string strISA02_AuthInfo = str10Spaces;
            private string strISA03_SecQaul = "00";
            private string strISA04_SecInfo = str10Spaces;
            private ISA_XQualifiers ISA05_SenderQual;
            private string strISA06_SenderID;//Must be 15 characters
            private ISA_XQualifiers ISA07_ReceiverQual;
            private string strISA08_ReceiverID;//Must be 15 characters
            private string strISA09_XDate = System.DateTime.Today.ToString("yyMMdd");
            private string strISA10_XTime = System.DateTime.Now.ToString("HHmm");
            private string strISA11_RepSep;
            private string strISA12_XVersion = "00501";
            private string strISA13_ControlNo;
            private string strISA14_Ack = "";
            private string strISA15_Usage;
            private string strISA16_EleSep;

            private string strDefaultSep;
            private string strFinishedSegment;
            private string strEOS;

            public string SetDefaultSep
            {
                set
                {
                    strDefaultSep = value;
                }
            }

            public string SetEOS
            {
                set
                {
                    strEOS = value;
                }
            }

            public string ISASegment
            {
                get
                {
                    return "ISA" + strDefaultSep + strISA01_AuthQual + strDefaultSep + strISA02_AuthInfo + strDefaultSep + strISA03_SecQaul + strDefaultSep + strISA04_SecInfo + strDefaultSep +
                           ReturnStringFromXQualValue(ISA05_SenderQual) + strDefaultSep + strISA06_SenderID + strDefaultSep + ReturnStringFromXQualValue(ISA07_ReceiverQual) + strDefaultSep + strISA08_ReceiverID + strDefaultSep + strISA09_XDate + strDefaultSep +
                            strISA10_XTime + strDefaultSep + strISA11_RepSep + strDefaultSep + strISA12_XVersion + strDefaultSep + strISA13_ControlNo + strDefaultSep + strISA14_Ack + strDefaultSep +
                            strISA15_Usage + strDefaultSep + strISA16_EleSep + strEOS;
                }
            }

            public enum ISA_XQualifiers
            {
                /// <remarks/>
                [System.Xml.Serialization.XmlEnumAttribute("01")]
                ItemDUNS,
                /// <remarks/>
                [System.Xml.Serialization.XmlEnumAttribute("08")]
                ItemUCC,
                /// <remarks/>
                [System.Xml.Serialization.XmlEnumAttribute("ZZ")]
                ItemMutual,
                /// <remarks/>
                [System.Xml.Serialization.XmlEnumAttribute("30")]
                ItemTIN,
            }

            public ISA_XQualifiers SenderQual
            {               
                set
                {
                    ISA05_SenderQual = value;
                }
            }

            public ISA_XQualifiers ReceiverQual
            {
                set
                {
                    ISA07_ReceiverQual = value;
                }
            }

            public ISA_XQualifiers ReturnXQualValueFromString(string strValue)
            {
                try
                {
                    switch (strValue)
                    {
                        case "01":
                            return ISA_XQualifiers.ItemDUNS;                            
                        case "08":
                            return ISA_XQualifiers.ItemUCC;                            
                        case "30":
                            return ISA_XQualifiers.ItemTIN;                            
                        default:
                            return ISA_XQualifiers.ItemMutual;  
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

            public string ReturnStringFromXQualValue(ISA_XQualifiers xEnum)
            {
                try
                {
                    switch(xEnum)
                    {
                        case ISA_XQualifiers.ItemDUNS:
                            return "01";
                        case ISA_XQualifiers.ItemMutual:
                            return "ZZ";
                        case ISA_XQualifiers.ItemTIN:
                            return "30";
                        case ISA_XQualifiers.ItemUCC:
                            return "08";
                        default:
                            return "ZZ";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

            public string SenderID
            {
                set
                {
                    strISA06_SenderID = (value + str15Spaces).ToString().Substring(0, 15);
                }
            }

            public string ReceiverID
            {
                set
                {
                    strISA08_ReceiverID = (value + str15Spaces).ToString().Substring(0, 15);
                }
            }

            public string RepSep
            {
                set
                {
                    strISA11_RepSep = value;
                }
            }

            public string ControlNo
            {
                set
                {
                    strISA13_ControlNo = value;
                }
            }

            public string Acknowledgement(bool blnAck)
            {
                try
                {
                    if (blnAck)
                        strISA14_Ack = "1";
                    else
                        strISA14_Ack = "";

                    return strISA14_Ack;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public string Usage
            {
                set
                {
                    strISA15_Usage = value;
                }
            }

            public string EleSep
            {
                set
                {
                    strISA16_EleSep = value;
                }
            }


        }

        public partial class GS_FunctionalGroupHeader
        {
            private GS_FunctionalIdentifierCodes IDCode;
            private string strSenderID;
            private string strReceiverID;
            private string strControlNo;
            private string strDate = System.DateTime.Today.ToString("yyyyMMdd");
            private string strTime = System.DateTime.Now.ToString("HHmm");
            private string strRACode = "X";
            private string strVersion = "005010X222A1";

            private string strDefaultSep;
            private string strFinishedSegment;
            private string strEOS;

            public string SetDefaultSep
            {
                set
                {
                    strDefaultSep = value;
                }
            }

            public string SetEOS
            {
                set
                {
                    strEOS = value;
                }
            }

            public enum GS_FunctionalIdentifierCodes
            {
                /// <remarks/>
                [System.Xml.Serialization.XmlEnumAttribute("HC")]
                ItemHCClaim,
            }

            public string SenderID
            {
                set
                {
                    strSenderID = value;
                }
            }

            public string ReceiverID
            {
                set
                {
                    strReceiverID = value;
                }
            }

            public string ControlNo
            {
                set
                {
                    strControlNo = value;
                }
            }

            public GS_FunctionalIdentifierCodes GSID
            {
                set
                {
                    IDCode = value;
                }
            }

            public GS_FunctionalIdentifierCodes GetIDCodeFromString(string strIDCode)
            {
                switch (strIDCode)
                {
                    case "HC":
                        return GS_FunctionalIdentifierCodes.ItemHCClaim;
                    default:
                        return GS_FunctionalIdentifierCodes.ItemHCClaim;
                }
            }

            public string GetStringFromIDCode(GS_FunctionalIdentifierCodes gsID)
            {
                switch (gsID)
                {
                    case GS_FunctionalIdentifierCodes.ItemHCClaim:
                        return "HC";
                    default:
                        return "HC";
                }
            }

            public string GS_Segment
            {
                get
                {
                    return "GS" + strDefaultSep + GetStringFromIDCode(IDCode) + strDefaultSep + strSenderID + strDefaultSep + strReceiverID + strDefaultSep + strDate + strDefaultSep +
                            strTime + strDefaultSep + strControlNo + strDefaultSep + strRACode + strDefaultSep + strVersion + strEOS;
                }
            }
        }

        public partial class GE_FunctionalGroupTrailer
        {
            private string strDefaultSep;
            private string strFinishedSegment;
            private string strEOS;
            private int intSTCount;
            private string strControlNo;
            
            public string ControlNo
            {
                set
                {
                    strControlNo = value;
                }
            }

            public string SetDefaultSep
            {
                set
                {
                    strDefaultSep = value;
                }
            }

            public string SetEOS
            {
                set
                {
                    strEOS = value;
                }
            }

            public int TransactionSetCount
            {
                set
                {
                    intSTCount = (int)value;
                }
            }

            public string GE_Segment
            {
                get
                {
                    return "GE" + strDefaultSep + intSTCount + strDefaultSep + strControlNo + strEOS;
                }
            }
        }

        public partial class IEA_InterchangeControlTrailer
        {
            private string strDefaultSep;
            private string strFinishedSegment;
            private string strEOS;
            private int intGSCount;
            private string strControlNo;

            public string ControlNo
            {
                set
                {
                    strControlNo = value;
                }
            }

            public string SetDefaultSep
            {
                set
                {
                    strDefaultSep = value;
                }
            }

            public string SetEOS
            {
                set
                {
                    strEOS = value;
                }
            }

            public int FunctionalGroupCount
            {
                set
                {
                    intGSCount = (int)value;
                }
            }

            public string IEA_Segment
            {
                get
                {
                    return "IEA" + strDefaultSep + intGSCount + strDefaultSep + strControlNo + strEOS;
                }
            }
        }

    }
}
