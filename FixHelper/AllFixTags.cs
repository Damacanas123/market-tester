using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixHelper
{
    //some groups are missing at the moment
    public class AllFixTags
    {

        public static string ORDER_ENTRY_CHANNEL_NAME = "BIST OE";
        public static string REF_DATA_CHANNEL_NAME = "BIST RD";
        public static string DROP_COPY_CHANNEL_NAME = "BIST DC";
        public struct Tag
        {
            public int TagNum;
            public string Name;
            //public string MessageType;
            //public string Channel;
            public string Type;
            //public string IsRequired;
            //public List<Tag> GroupElements;
            //public Tag(int tagNum, string name, string messageType, string channel, string isRequired, string type)
            //{
            //    TagNum = tagNum;
            //    Name = name;
            //    MessageType = messageType;
            //    Channel = channel;
            //    Type = type;
            //    IsRequired = isRequired;
            //    GroupElements = null;
            //}
            //public Tag(int tagNum, string name, string messageType, string channel, string isRequired, string type, List<Tag> groupElements)
            //{
            //    TagNum = tagNum;
            //    Name = name;
            //    MessageType = messageType;
            //    Channel = channel;
            //    Type = type;
            //    IsRequired = isRequired;
            //    GroupElements = groupElements;
            //}
            public Tag (int tagNum,string name,string type)
            {
                this.TagNum = tagNum;
                this.Name = name;
                this.Type = type;
            }
        }

        private static AllFixTags instance = null;
        private List<Tag> tags;
        //channel name -> message type -> list of Tag struct
        
        
        //keys : tags values : Tag instances
        //note that only top level tags are ınserted into this collection
        public Dictionary<int, Tag> tagToObjectMap;
        public Dictionary<int, Tag> headerTagToObjectMap;
        public Dictionary<int, Tag> allTagToObjectMap = new Dictionary<int, Tag>();
        public Dictionary<int,Dictionary<string,string>> msgValueMap;

        public static AllFixTags GetInstance()
        {
            if (instance == null)
            {
                instance = new AllFixTags();
            }
            return instance;
        }

        public string GetValueExplanation(int tag,string value)
        {
            if(msgValueMap.TryGetValue(tag,out Dictionary<string,string> valueMap))
            {
                if(valueMap.TryGetValue(value,out string valueExplanation))
                {
                    return valueExplanation;
                }
            }
            return null;
        }

        public string GetTagExplanation(int tag)
        {
            if(allTagToObjectMap.TryGetValue(tag,out Tag tagObject))
            {
                return tagObject.Name;
            }
            return null;
        }

        private AllFixTags()
        {
            tags = new List<Tag>()
            {
                new Tag(1,"Account","String"),
                new Tag(2,"AdvId","String"),
                new Tag(3,"AdvRefID","String"),
                new Tag(4,"AdvSide","char"),
                new Tag(5,"AdvTransType","String"),
                new Tag(6,"AvgPx","Price"),
                new Tag(7,"BeginSeqNo","SeqNum"),
                new Tag(8,"BeginString","String"),
                new Tag(9,"BodyLength","Length"),
                new Tag(10,"CheckSum","String"),
                new Tag(11,"ClOrdID","String"),
                new Tag(12,"Commission","Amt"),
                new Tag(13,"CommType","char"),
                new Tag(14,"CumQty","Qty"),
                new Tag(15,"Currency","Currency"),
                new Tag(16,"EndSeqNo","SeqNum"),
                new Tag(17,"ExecID","String"),
                new Tag(18,"ExecInst","MultipleCharValue"),
                new Tag(19,"ExecRefID","String"),
                new Tag(20,"ExecTransType","char"),
                new Tag(21,"HandlInst","char"),
                new Tag(22,"SecurityIDSource","String"),
                new Tag(23,"IOIID","String"),
                new Tag(24,"IOIOthSvc","char"),
                new Tag(25,"IOIQltyInd","char"),
                new Tag(26,"IOIRefID","String"),
                new Tag(27,"IOIQty","String"),
                new Tag(28,"IOITransType","char"),
                new Tag(29,"LastCapacity","char"),
                new Tag(30,"LastMkt","Exchange"),
                new Tag(31,"LastPx","Price"),
                new Tag(32,"LastQty","Qty"),
                new Tag(33,"NoLinesOfText","NumInGroup"),
                new Tag(34,"MsgSeqNum","SeqNum"),
                new Tag(35,"MsgType","String"),
                new Tag(36,"NewSeqNo","SeqNum"),
                new Tag(37,"OrderID","String"),
                new Tag(38,"OrderQty","Qty"),
                new Tag(39,"OrdStatus","char"),
                new Tag(40,"OrdType","char"),
                new Tag(41,"OrigClOrdID","String"),
                new Tag(42,"OrigTime","UTCTimestamp"),
                new Tag(43,"PossDupFlag","Boolean"),
                new Tag(44,"Price","Price"),
                new Tag(45,"RefSeqNum","SeqNum"),
                new Tag(46,"RelatdSym","String"),
                new Tag(47,"Rule80A(No","char"),
                new Tag(48,"SecurityID","String"),
                new Tag(49,"SenderCompID","String"),
                new Tag(50,"SenderSubID","String"),
                new Tag(51,"SendingDate","LocalMktDate"),
                new Tag(52,"SendingTime","UTCTimestamp"),
                new Tag(53,"Quantity","Qty"),
                new Tag(54,"Side","char"),
                new Tag(55,"Symbol","String"),
                new Tag(56,"TargetCompID","String"),
                new Tag(57,"TargetSubID","String"),
                new Tag(58,"Text","String"),
                new Tag(59,"TimeInForce","char"),
                new Tag(60,"TransactTime","UTCTimestamp"),
                new Tag(61,"Urgency","char"),
                new Tag(62,"ValidUntilTime","UTCTimestamp"),
                new Tag(63,"SettlType","String"),
                new Tag(64,"SettlDate","LocalMktDate"),
                new Tag(65,"SymbolSfx","String"),
                new Tag(66,"ListID","String"),
                new Tag(67,"ListSeqNo","int"),
                new Tag(68,"TotNoOrders","int"),
                new Tag(69,"ListExecInst","String"),
                new Tag(70,"AllocID","String"),
                new Tag(71,"AllocTransType","char"),
                new Tag(72,"RefAllocID","String"),
                new Tag(73,"NoOrders","NumInGroup"),
                new Tag(74,"AvgPxPrecision","int"),
                new Tag(75,"TradeDate","LocalMktDate"),
                new Tag(76,"ExecBroker","String"),
                new Tag(77,"PositionEffect","char"),
                new Tag(78,"NoAllocs","NumInGroup"),
                new Tag(79,"AllocAccount","String"),
                new Tag(80,"AllocQty","Qty"),
                new Tag(81,"ProcessCode","char"),
                new Tag(82,"NoRpts","int"),
                new Tag(83,"RptSeq","int"),
                new Tag(84,"CxlQty","Qty"),
                new Tag(85,"NoDlvyInst","NumInGroup"),
                new Tag(86,"DlvyInst","String"),
                new Tag(87,"AllocStatus","int"),
                new Tag(88,"AllocRejCode","int"),
                new Tag(89,"Signature","data"),
                new Tag(90,"SecureDataLen","Length"),
                new Tag(91,"SecureData","data"),
                new Tag(92,"BrokerOfCredit","String"),
                new Tag(93,"SignatureLength","Length"),
                new Tag(94,"EmailType","char"),
                new Tag(95,"RawDataLength","Length"),
                new Tag(96,"RawData","data"),
                new Tag(97,"PossResend","Boolean"),
                new Tag(98,"EncryptMethod","int"),
                new Tag(99,"StopPx","Price"),
                new Tag(100,"ExDestination","Exchange"),
                new Tag(101,"(Not","n/a"),
                new Tag(102,"CxlRejReason","int"),
                new Tag(103,"OrdRejReason","int"),
                new Tag(104,"IOIQualifier","char"),
                new Tag(105,"WaveNo","String"),
                new Tag(106,"Issuer","String"),
                new Tag(107,"SecurityDesc","String"),
                new Tag(108,"HeartBtInt","int"),
                new Tag(109,"ClientID","String"),
                new Tag(110,"MinQty","Qty"),
                new Tag(111,"MaxFloor","Qty"),
                new Tag(112,"TestReqID","String"),
                new Tag(113,"ReportToExch","Boolean"),
                new Tag(114,"LocateReqd","Boolean"),
                new Tag(115,"OnBehalfOfCompID","String"),
                new Tag(116,"OnBehalfOfSubID","String"),
                new Tag(117,"QuoteID","String"),
                new Tag(118,"NetMoney","Amt"),
                new Tag(119,"SettlCurrAmt","Amt"),
                new Tag(120,"SettlCurrency","Currency"),
                new Tag(121,"ForexReq","Boolean"),
                new Tag(122,"OrigSendingTime","UTCTimestamp"),
                new Tag(123,"GapFillFlag","Boolean"),
                new Tag(124,"NoExecs","NumInGroup"),
                new Tag(125,"CxlType","char"),
                new Tag(126,"ExpireTime","UTCTimestamp"),
                new Tag(127,"DKReason","char"),
                new Tag(128,"DeliverToCompID","String"),
                new Tag(129,"DeliverToSubID","String"),
                new Tag(130,"IOINaturalFlag","Boolean"),
                new Tag(131,"QuoteReqID","String"),
                new Tag(132,"BidPx","Price"),
                new Tag(133,"OfferPx","Price"),
                new Tag(134,"BidSize","Qty"),
                new Tag(135,"OfferSize","Qty"),
                new Tag(136,"NoMiscFees","NumInGroup"),
                new Tag(137,"MiscFeeAmt","Amt"),
                new Tag(138,"MiscFeeCurr","Currency"),
                new Tag(139,"MiscFeeType","String"),
                new Tag(140,"PrevClosePx","Price"),
                new Tag(141,"ResetSeqNumFlag","Boolean"),
                new Tag(142,"SenderLocationID","String"),
                new Tag(143,"TargetLocationID","String"),
                new Tag(144,"OnBehalfOfLocationID","String"),
                new Tag(145,"DeliverToLocationID","String"),
                new Tag(146,"NoRelatedSym","NumInGroup"),
                new Tag(147,"Subject","String"),
                new Tag(148,"Headline","String"),
                new Tag(149,"URLLink","String"),
                new Tag(150,"ExecType","char"),
                new Tag(151,"LeavesQty","Qty"),
                new Tag(152,"CashOrderQty","Qty"),
                new Tag(153,"AllocAvgPx","Price"),
                new Tag(154,"AllocNetMoney","Amt"),
                new Tag(155,"SettlCurrFxRate","float"),
                new Tag(156,"SettlCurrFxRateCalc","char"),
                new Tag(157,"NumDaysInterest","int"),
                new Tag(158,"AccruedInterestRate","Percentage"),
                new Tag(159,"AccruedInterestAmt","Amt"),
                new Tag(160,"SettlInstMode","char"),
                new Tag(161,"AllocText","String"),
                new Tag(162,"SettlInstID","String"),
                new Tag(163,"SettlInstTransType","char"),
                new Tag(164,"EmailThreadID","String"),
                new Tag(165,"SettlInstSource","char"),
                new Tag(166,"SettlLocation","String"),
                new Tag(167,"SecurityType","String"),
                new Tag(168,"EffectiveTime","UTCTimestamp"),
                new Tag(169,"StandInstDbType","int"),
                new Tag(170,"StandInstDbName","String"),
                new Tag(171,"StandInstDbID","String"),
                new Tag(172,"SettlDeliveryType","int"),
                new Tag(173,"SettlDepositoryCode","String"),
                new Tag(174,"SettlBrkrCode","String"),
                new Tag(175,"SettlInstCode","String"),
                new Tag(176,"SecuritySettlAgentName","String"),
                new Tag(177,"SecuritySettlAgentCode","String"),
                new Tag(178,"SecuritySettlAgentAcctNum","String"),
                new Tag(179,"SecuritySettlAgentAcctName","String"),
                new Tag(180,"SecuritySettlAgentContactName","String"),
                new Tag(181,"SecuritySettlAgentContactPhone","String"),
                new Tag(182,"CashSettlAgentName","String"),
                new Tag(183,"CashSettlAgentCode","String"),
                new Tag(184,"CashSettlAgentAcctNum","String"),
                new Tag(185,"CashSettlAgentAcctName","String"),
                new Tag(186,"CashSettlAgentContactName","String"),
                new Tag(187,"CashSettlAgentContactPhone","String"),
                new Tag(188,"BidSpotRate","Price"),
                new Tag(189,"BidForwardPoints","PriceOffset"),
                new Tag(190,"OfferSpotRate","Price"),
                new Tag(191,"OfferForwardPoints","PriceOffset"),
                new Tag(192,"OrderQty2","Qty"),
                new Tag(193,"SettlDate2","LocalMktDate"),
                new Tag(194,"LastSpotRate","Price"),
                new Tag(195,"LastForwardPoints","PriceOffset"),
                new Tag(196,"AllocLinkID","String"),
                new Tag(197,"AllocLinkType","int"),
                new Tag(198,"SecondaryOrderID","String"),
                new Tag(199,"NoIOIQualifiers","NumInGroup"),
                new Tag(200,"MaturityMonthYear","MonthYear"),
                new Tag(201,"PutOrCall","int"),
                new Tag(202,"StrikePrice","Price"),
                new Tag(203,"CoveredOrUncovered","int"),
                new Tag(204,"CustomerOrFirm","int"),
                new Tag(205,"MaturityDay","day-of-month"),
                new Tag(206,"OptAttribute","char"),
                new Tag(207,"SecurityExchange","Exchange"),
                new Tag(208,"NotifyBrokerOfCredit","Boolean"),
                new Tag(209,"AllocHandlInst","int"),
                new Tag(210,"MaxShow","Qty"),
                new Tag(211,"PegOffsetValue","float"),
                new Tag(212,"XmlDataLen","Length"),
                new Tag(213,"XmlData","data"),
                new Tag(214,"SettlInstRefID","String"),
                new Tag(215,"NoRoutingIDs","NumInGroup"),
                new Tag(216,"RoutingType","int"),
                new Tag(217,"RoutingID","String"),
                new Tag(218,"Spread","PriceOffset"),
                new Tag(219,"Benchmark","char"),
                new Tag(220,"BenchmarkCurveCurrency","Currency"),
                new Tag(221,"BenchmarkCurveName","String"),
                new Tag(222,"BenchmarkCurvePoint","String"),
                new Tag(223,"CouponRate","Percentage"),
                new Tag(224,"CouponPaymentDate","LocalMktDate"),
                new Tag(225,"IssueDate","LocalMktDate"),
                new Tag(226,"RepurchaseTerm","int"),
                new Tag(227,"RepurchaseRate","Percentage"),
                new Tag(228,"Factor","float"),
                new Tag(229,"TradeOriginationDate","LocalMktDate"),
                new Tag(230,"ExDate","LocalMktDate"),
                new Tag(231,"ContractMultiplier","float"),
                new Tag(232,"NoStipulations","NumInGroup"),
                new Tag(233,"StipulationType","String"),
                new Tag(234,"StipulationValue","String"),
                new Tag(235,"YieldType","String"),
                new Tag(236,"Yield","Percentage"),
                new Tag(237,"TotalTakedown","Amt"),
                new Tag(238,"Concession","Amt"),
                new Tag(239,"RepoCollateralSecurityType","int"),
                new Tag(240,"RedemptionDate","LocalMktDate"),
                new Tag(241,"UnderlyingCouponPaymentDate","LocalMktDate"),
                new Tag(242,"UnderlyingIssueDate","LocalMktDate"),
                new Tag(243,"UnderlyingRepoCollateralSecurityType","int"),
                new Tag(244,"UnderlyingRepurchaseTerm","int"),
                new Tag(245,"UnderlyingRepurchaseRate","Percentage"),
                new Tag(246,"UnderlyingFactor","float"),
                new Tag(247,"UnderlyingRedemptionDate","LocalMktDate"),
                new Tag(248,"LegCouponPaymentDate","LocalMktDate"),
                new Tag(249,"LegIssueDate","LocalMktDate"),
                new Tag(250,"LegRepoCollateralSecurityType","int"),
                new Tag(251,"LegRepurchaseTerm","int"),
                new Tag(252,"LegRepurchaseRate","Percentage"),
                new Tag(253,"LegFactor","float"),
                new Tag(254,"LegRedemptionDate","LocalMktDate"),
                new Tag(255,"CreditRating","String"),
                new Tag(256,"UnderlyingCreditRating","String"),
                new Tag(257,"LegCreditRating","String"),
                new Tag(258,"TradedFlatSwitch","Boolean"),
                new Tag(259,"BasisFeatureDate","LocalMktDate"),
                new Tag(260,"BasisFeaturePrice","Price"),
                new Tag(262,"MDReqID","String"),
                new Tag(263,"SubscriptionRequestType","char"),
                new Tag(264,"MarketDepth","int"),
                new Tag(265,"MDUpdateType","int"),
                new Tag(266,"AggregatedBook","Boolean"),
                new Tag(267,"NoMDEntryTypes","NumInGroup"),
                new Tag(268,"NoMDEntries","NumInGroup"),
                new Tag(269,"MDEntryType","char"),
                new Tag(270,"MDEntryPx","Price"),
                new Tag(271,"MDEntrySize","Qty"),
                new Tag(272,"MDEntryDate","UTCDateOnly"),
                new Tag(273,"MDEntryTime","UTCTimeOnly"),
                new Tag(274,"TickDirection","char"),
                new Tag(275,"MDMkt","Exchange"),
                new Tag(276,"QuoteCondition","MultipleStringValue"),
                new Tag(277,"TradeCondition","MultipleStringValue"),
                new Tag(278,"MDEntryID","String"),
                new Tag(279,"MDUpdateAction","char"),
                new Tag(280,"MDEntryRefID","String"),
                new Tag(281,"MDReqRejReason","char"),
                new Tag(282,"MDEntryOriginator","String"),
                new Tag(283,"LocationID","String"),
                new Tag(284,"DeskID","String"),
                new Tag(285,"DeleteReason","char"),
                new Tag(286,"OpenCloseSettlFlag","MultipleCharValue"),
                new Tag(287,"SellerDays","int"),
                new Tag(288,"MDEntryBuyer","String"),
                new Tag(289,"MDEntrySeller","String"),
                new Tag(290,"MDEntryPositionNo","int"),
                new Tag(291,"FinancialStatus","MultipleCharValue"),
                new Tag(292,"CorporateAction","MultipleCharValue"),
                new Tag(293,"DefBidSize","Qty"),
                new Tag(294,"DefOfferSize","Qty"),
                new Tag(295,"NoQuoteEntries","NumInGroup"),
                new Tag(296,"NoQuoteSets","NumInGroup"),
                new Tag(297,"QuoteStatus","int"),
                new Tag(298,"QuoteCancelType","int"),
                new Tag(299,"QuoteEntryID","String"),
                new Tag(300,"QuoteRejectReason","int"),
                new Tag(301,"QuoteResponseLevel","int"),
                new Tag(302,"QuoteSetID","String"),
                new Tag(303,"QuoteRequestType","int"),
                new Tag(304,"TotNoQuoteEntries","int"),
                new Tag(305,"UnderlyingSecurityIDSource","String"),
                new Tag(306,"UnderlyingIssuer","String"),
                new Tag(307,"UnderlyingSecurityDesc","String"),
                new Tag(308,"UnderlyingSecurityExchange","Exchange"),
                new Tag(309,"UnderlyingSecurityID","String"),
                new Tag(310,"UnderlyingSecurityType","String"),
                new Tag(311,"UnderlyingSymbol","String"),
                new Tag(312,"UnderlyingSymbolSfx","String"),
                new Tag(313,"UnderlyingMaturityMonthYear","MonthYear"),
                new Tag(314,"UnderlyingMaturityDay","day-of-month"),
                new Tag(315,"UnderlyingPutOrCall","int"),
                new Tag(316,"UnderlyingStrikePrice","Price"),
                new Tag(317,"UnderlyingOptAttribute","char"),
                new Tag(318,"UnderlyingCurrency","Currency"),
                new Tag(319,"RatioQty","Qty"),
                new Tag(320,"SecurityReqID","String"),
                new Tag(321,"SecurityRequestType","int"),
                new Tag(322,"SecurityResponseID","String"),
                new Tag(323,"SecurityResponseType","int"),
                new Tag(324,"SecurityStatusReqID","String"),
                new Tag(325,"UnsolicitedIndicator","Boolean"),
                new Tag(326,"SecurityTradingStatus","int"),
                new Tag(327,"HaltReason","int"),
                new Tag(328,"InViewOfCommon","Boolean"),
                new Tag(329,"DueToRelated","Boolean"),
                new Tag(330,"BuyVolume","Qty"),
                new Tag(331,"SellVolume","Qty"),
                new Tag(332,"HighPx","Price"),
                new Tag(333,"LowPx","Price"),
                new Tag(334,"Adjustment","int"),
                new Tag(335,"TradSesReqID","String"),
                new Tag(336,"TradingSessionID","String"),
                new Tag(337,"ContraTrader","String"),
                new Tag(338,"TradSesMethod","int"),
                new Tag(339,"TradSesMode","int"),
                new Tag(340,"TradSesStatus","int"),
                new Tag(341,"TradSesStartTime","UTCTimestamp"),
                new Tag(342,"TradSesOpenTime","UTCTimestamp"),
                new Tag(343,"TradSesPreCloseTime","UTCTimestamp"),
                new Tag(344,"TradSesCloseTime","UTCTimestamp"),
                new Tag(345,"TradSesEndTime","UTCTimestamp"),
                new Tag(346,"NumberOfOrders","int"),
                new Tag(347,"MessageEncoding","String"),
                new Tag(348,"EncodedIssuerLen","Length"),
                new Tag(349,"EncodedIssuer","data"),
                new Tag(350,"EncodedSecurityDescLen","Length"),
                new Tag(351,"EncodedSecurityDesc","data"),
                new Tag(352,"EncodedListExecInstLen","Length"),
                new Tag(353,"EncodedListExecInst","data"),
                new Tag(354,"EncodedTextLen","Length"),
                new Tag(355,"EncodedText","data"),
                new Tag(356,"EncodedSubjectLen","Length"),
                new Tag(357,"EncodedSubject","data"),
                new Tag(358,"EncodedHeadlineLen","Length"),
                new Tag(359,"EncodedHeadline","data"),
                new Tag(360,"EncodedAllocTextLen","Length"),
                new Tag(361,"EncodedAllocText","data"),
                new Tag(362,"EncodedUnderlyingIssuerLen","Length"),
                new Tag(363,"EncodedUnderlyingIssuer","data"),
                new Tag(364,"EncodedUnderlyingSecurityDescLen","Length"),
                new Tag(365,"EncodedUnderlyingSecurityDesc","data"),
                new Tag(366,"AllocPrice","Price"),
                new Tag(367,"QuoteSetValidUntilTime","UTCTimestamp"),
                new Tag(368,"QuoteEntryRejectReason","int"),
                new Tag(369,"LastMsgSeqNumProcessed","SeqNum"),
                new Tag(370,"OnBehalfOfSendingTime","UTCTimestamp"),
                new Tag(371,"RefTagID","int"),
                new Tag(372,"RefMsgType","String"),
                new Tag(373,"SessionRejectReason","int"),
                new Tag(374,"BidRequestTransType","char"),
                new Tag(375,"ContraBroker","String"),
                new Tag(376,"ComplianceID","String"),
                new Tag(377,"SolicitedFlag","Boolean"),
                new Tag(378,"ExecRestatementReason","int"),
                new Tag(379,"BusinessRejectRefID","String"),
                new Tag(380,"BusinessRejectReason","int"),
                new Tag(381,"GrossTradeAmt","Amt"),
                new Tag(382,"NoContraBrokers","NumInGroup"),
                new Tag(383,"MaxMessageSize","Length"),
                new Tag(384,"NoMsgTypes","NumInGroup"),
                new Tag(385,"MsgDirection","char"),
                new Tag(386,"NoTradingSessions","NumInGroup"),
                new Tag(387,"TotalVolumeTraded","Qty"),
                new Tag(388,"DiscretionInst","char"),
                new Tag(389,"DiscretionOffsetValue","float"),
                new Tag(390,"BidID","String"),
                new Tag(391,"ClientBidID","String"),
                new Tag(392,"ListName","String"),
                new Tag(393,"TotNoRelatedSym","int"),
                new Tag(394,"BidType","int"),
                new Tag(395,"NumTickets","int"),
                new Tag(396,"SideValue1","Amt"),
                new Tag(397,"SideValue2","Amt"),
                new Tag(398,"NoBidDescriptors","NumInGroup"),
                new Tag(399,"BidDescriptorType","int"),
                new Tag(400,"BidDescriptor","String"),
                new Tag(401,"SideValueInd","int"),
                new Tag(402,"LiquidityPctLow","Percentage"),
                new Tag(403,"LiquidityPctHigh","Percentage"),
                new Tag(404,"LiquidityValue","Amt"),
                new Tag(405,"EFPTrackingError","Percentage"),
                new Tag(406,"FairValue","Amt"),
                new Tag(407,"OutsideIndexPct","Percentage"),
                new Tag(408,"ValueOfFutures","Amt"),
                new Tag(409,"LiquidityIndType","int"),
                new Tag(410,"WtAverageLiquidity","Percentage"),
                new Tag(411,"ExchangeForPhysical","Boolean"),
                new Tag(412,"OutMainCntryUIndex","Amt"),
                new Tag(413,"CrossPercent","Percentage"),
                new Tag(414,"ProgRptReqs","int"),
                new Tag(415,"ProgPeriodInterval","int"),
                new Tag(416,"IncTaxInd","int"),
                new Tag(417,"NumBidders","int"),
                new Tag(418,"BidTradeType","char"),
                new Tag(419,"BasisPxType","char"),
                new Tag(420,"NoBidComponents","NumInGroup"),
                new Tag(421,"Country","Country"),
                new Tag(422,"TotNoStrikes","int"),
                new Tag(423,"PriceType","int"),
                new Tag(424,"DayOrderQty","Qty"),
                new Tag(425,"DayCumQty","Qty"),
                new Tag(426,"DayAvgPx","Price"),
                new Tag(427,"GTBookingInst","int"),
                new Tag(428,"NoStrikes","NumInGroup"),
                new Tag(429,"ListStatusType","int"),
                new Tag(430,"NetGrossInd","int"),
                new Tag(431,"ListOrderStatus","int"),
                new Tag(432,"ExpireDate","LocalMktDate"),
                new Tag(433,"ListExecInstType","char"),
                new Tag(434,"CxlRejResponseTo","char"),
                new Tag(435,"UnderlyingCouponRate","Percentage"),
                new Tag(436,"UnderlyingContractMultiplier","float"),
                new Tag(437,"ContraTradeQty","Qty"),
                new Tag(438,"ContraTradeTime","UTCTimestamp"),
                new Tag(439,"ClearingFirm","String"),
                new Tag(440,"ClearingAccount","String"),
                new Tag(441,"LiquidityNumSecurities","int"),
                new Tag(442,"MultiLegReportingType","char"),
                new Tag(443,"StrikeTime","UTCTimestamp"),
                new Tag(444,"ListStatusText","String"),
                new Tag(445,"EncodedListStatusTextLen","Length"),
                new Tag(446,"EncodedListStatusText","data"),
                new Tag(447,"PartyIDSource","char"),
                new Tag(448,"PartyID","String"),
                new Tag(449,"TotalVolumeTradedDate","UTCDateOnly"),
                new Tag(450,"TotalVolumeTraded","UTCTimeOnly"),
                new Tag(451,"NetChgPrevDay","PriceOffset"),
                new Tag(452,"PartyRole","int"),
                new Tag(453,"NoPartyIDs","NumInGroup"),
                new Tag(454,"NoSecurityAltID","NumInGroup"),
                new Tag(455,"SecurityAltID","String"),
                new Tag(456,"SecurityAltIDSource","String"),
                new Tag(457,"NoUnderlyingSecurityAltID","NumInGroup"),
                new Tag(458,"UnderlyingSecurityAltID","String"),
                new Tag(459,"UnderlyingSecurityAltIDSource","String"),
                new Tag(460,"Product","int"),
                new Tag(461,"CFICode","String"),
                new Tag(462,"UnderlyingProduct","int"),
                new Tag(463,"UnderlyingCFICode","String"),
                new Tag(464,"TestMessageIndicator","Boolean"),
                new Tag(465,"QuantityType","int"),
                new Tag(466,"BookingRefID","String"),
                new Tag(467,"IndividualAllocID","String"),
                new Tag(468,"RoundingDirection","char"),
                new Tag(469,"RoundingModulus","float"),
                new Tag(470,"CountryOfIssue","Country"),
                new Tag(471,"StateOrProvinceOfIssue","String"),
                new Tag(472,"LocaleOfIssue","String"),
                new Tag(473,"NoRegistDtls","NumInGroup"),
                new Tag(474,"MailingDtls","String"),
                new Tag(475,"InvestorCountryOfResidence","Country"),
                new Tag(476,"PaymentRef","String"),
                new Tag(477,"DistribPaymentMethod","int"),
                new Tag(478,"CashDistribCurr","Currency"),
                new Tag(479,"CommCurrency","Currency"),
                new Tag(480,"CancellationRights","char"),
                new Tag(481,"MoneyLaunderingStatus","char"),
                new Tag(482,"MailingInst","String"),
                new Tag(483,"TransBkdTime","UTCTimestamp"),
                new Tag(484,"ExecPriceType","char"),
                new Tag(485,"ExecPriceAdjustment","float"),
                new Tag(486,"DateOfBirth","LocalMktDate"),
                new Tag(487,"TradeReportTransType","int"),
                new Tag(488,"CardHolderName","String"),
                new Tag(489,"CardNumber","String"),
                new Tag(490,"CardExpDate","LocalMktDate"),
                new Tag(491,"CardIssNum","String"),
                new Tag(492,"PaymentMethod","int"),
                new Tag(493,"RegistAcctType","String"),
                new Tag(494,"Designation","String"),
                new Tag(495,"TaxAdvantageType","int"),
                new Tag(496,"RegistRejReasonText","String"),
                new Tag(497,"FundRenewWaiv","char"),
                new Tag(498,"CashDistribAgentName","String"),
                new Tag(499,"CashDistribAgentCode","String"),
                new Tag(500,"CashDistribAgentAcctNumber","String"),
                new Tag(501,"CashDistribPayRef","String"),
                new Tag(502,"CashDistribAgentAcctName","String"),
                new Tag(503,"CardStartDate","LocalMktDate"),
                new Tag(504,"PaymentDate","LocalMktDate"),
                new Tag(505,"PaymentRemitterID","String"),
                new Tag(506,"RegistStatus","char"),
                new Tag(507,"RegistRejReasonCode","int"),
                new Tag(508,"RegistRefID","String"),
                new Tag(509,"RegistDtls","String"),
                new Tag(510,"NoDistribInsts","NumInGroup"),
                new Tag(511,"RegistEmail","String"),
                new Tag(512,"DistribPercentage","Percentage"),
                new Tag(513,"RegistID","String"),
                new Tag(514,"RegistTransType","char"),
                new Tag(515,"ExecValuationPoint","UTCTimestamp"),
                new Tag(516,"OrderPercent","Percentage"),
                new Tag(517,"OwnershipType","char"),
                new Tag(518,"NoContAmts","NumInGroup"),
                new Tag(519,"ContAmtType","int"),
                new Tag(520,"ContAmtValue","float"),
                new Tag(521,"ContAmtCurr","Currency"),
                new Tag(522,"OwnerType","int"),
                new Tag(523,"PartySubID","String"),
                new Tag(524,"NestedPartyID","String"),
                new Tag(525,"NestedPartyIDSource","char"),
                new Tag(526,"SecondaryClOrdID","String"),
                new Tag(527,"SecondaryExecID","String"),
                new Tag(528,"OrderCapacity","char"),
                new Tag(529,"OrderRestrictions","MultipleCharValue"),
                new Tag(530,"MassCancelRequestType","char"),
                new Tag(531,"MassCancelResponse","char"),
                new Tag(532,"MassCancelRejectReason","int"),
                new Tag(533,"TotalAffectedOrders","int"),
                new Tag(534,"NoAffectedOrders","NumInGroup"),
                new Tag(535,"AffectedOrderID","String"),
                new Tag(536,"AffectedSecondaryOrderID","String"),
                new Tag(537,"QuoteType","int"),
                new Tag(538,"NestedPartyRole","int"),
                new Tag(539,"NoNestedPartyIDs","NumInGroup"),
                new Tag(540,"TotalAccruedInterestAmt","Amt"),
                new Tag(541,"MaturityDate","LocalMktDate"),
                new Tag(542,"UnderlyingMaturityDate","LocalMktDate"),
                new Tag(543,"InstrRegistry","String"),
                new Tag(544,"CashMargin","char"),
                new Tag(545,"NestedPartySubID","String"),
                new Tag(546,"Scope","MultipleCharValue"),
                new Tag(547,"MDImplicitDelete","Boolean"),
                new Tag(548,"CrossID","String"),
                new Tag(549,"CrossType","int"),
                new Tag(550,"CrossPrioritization","int"),
                new Tag(551,"OrigCrossID","String"),
                new Tag(552,"NoSides","NumInGroup"),
                new Tag(553,"Username","String"),
                new Tag(554,"Password","String"),
                new Tag(555,"NoLegs","NumInGroup"),
                new Tag(556,"LegCurrency","Currency"),
                new Tag(557,"TotNoSecurityTypes","int"),
                new Tag(558,"NoSecurityTypes","NumInGroup"),
                new Tag(559,"SecurityListRequestType","int"),
                new Tag(560,"SecurityRequestResult","int"),
                new Tag(561,"RoundLot","Qty"),
                new Tag(562,"MinTradeVol","Qty"),
                new Tag(563,"MultiLegRptTypeReq","int"),
                new Tag(564,"LegPositionEffect","char"),
                new Tag(565,"LegCoveredOrUncovered","int"),
                new Tag(566,"LegPrice","Price"),
                new Tag(567,"TradSesStatusRejReason","int"),
                new Tag(568,"TradeRequestID","String"),
                new Tag(569,"TradeRequestType","int"),
                new Tag(570,"PreviouslyReported","Boolean"),
                new Tag(571,"TradeReportID","String"),
                new Tag(572,"TradeReportRefID","String"),
                new Tag(573,"MatchStatus","char"),
                new Tag(574,"MatchType","String"),
                new Tag(575,"OddLot","Boolean"),
                new Tag(576,"NoClearingInstructions","NumInGroup"),
                new Tag(577,"ClearingInstruction","int"),
                new Tag(578,"TradeInputSource","String"),
                new Tag(579,"TradeInputDevice","String"),
                new Tag(580,"NoDates","int"),
                new Tag(581,"AccountType","int"),
                new Tag(582,"CustOrderCapacity","int"),
                new Tag(583,"ClOrdLinkID","String"),
                new Tag(584,"MassStatusReqID","String"),
                new Tag(585,"MassStatusReqType","int"),
                new Tag(586,"OrigOrdModTime","UTCTimestamp"),
                new Tag(587,"LegSettlType","char"),
                new Tag(588,"LegSettlDate","LocalMktDate"),
                new Tag(589,"DayBookingInst","char"),
                new Tag(590,"BookingUnit","char"),
                new Tag(591,"PreallocMethod","char"),
                new Tag(592,"UnderlyingCountryOfIssue","Country"),
                new Tag(593,"UnderlyingStateOrProvinceOfIssue","String"),
                new Tag(594,"UnderlyingLocaleOfIssue","String"),
                new Tag(595,"UnderlyingInstrRegistry","String"),
                new Tag(596,"LegCountryOfIssue","Country"),
                new Tag(597,"LegStateOrProvinceOfIssue","String"),
                new Tag(598,"LegLocaleOfIssue","String"),
                new Tag(599,"LegInstrRegistry","String"),
                new Tag(600,"LegSymbol","String"),
                new Tag(601,"LegSymbolSfx","String"),
                new Tag(602,"LegSecurityID","String"),
                new Tag(603,"LegSecurityIDSource","String"),
                new Tag(604,"NoLegSecurityAltID","String"),
                new Tag(605,"LegSecurityAltID","String"),
                new Tag(606,"LegSecurityAltIDSource","String"),
                new Tag(607,"LegProduct","int"),
                new Tag(608,"LegCFICode","String"),
                new Tag(609,"LegSecurityType","String"),
                new Tag(610,"LegMaturityMonthYear","MonthYear"),
                new Tag(611,"LegMaturityDate","LocalMktDate"),
                new Tag(612,"LegStrikePrice","Price"),
                new Tag(613,"LegOptAttribute","char"),
                new Tag(614,"LegContractMultiplier","float"),
                new Tag(615,"LegCouponRate","Percentage"),
                new Tag(616,"LegSecurityExchange","Exchange"),
                new Tag(617,"LegIssuer","String"),
                new Tag(618,"EncodedLegIssuerLen","Length"),
                new Tag(619,"EncodedLegIssuer","data"),
                new Tag(620,"LegSecurityDesc","String"),
                new Tag(621,"EncodedLegSecurityDescLen","Length"),
                new Tag(622,"EncodedLegSecurityDesc","data"),
                new Tag(623,"LegRatioQty","float"),
                new Tag(624,"LegSide","char"),
                new Tag(625,"TradingSessionSubID","String"),
                new Tag(626,"AllocType","int"),
                new Tag(627,"NoHops","NumInGroup"),
                new Tag(628,"HopCompID","String"),
                new Tag(629,"HopSendingTime","UTCTimestamp"),
                new Tag(630,"HopRefID","SeqNum"),
                new Tag(631,"MidPx","Price"),
                new Tag(632,"BidYield","Percentage"),
                new Tag(633,"MidYield","Percentage"),
                new Tag(634,"OfferYield","Percentage"),
                new Tag(635,"ClearingFeeIndicator","String"),
                new Tag(636,"WorkingIndicator","Boolean"),
                new Tag(637,"LegLastPx","Price"),
                new Tag(638,"PriorityIndicator","int"),
                new Tag(639,"PriceImprovement","PriceOffset"),
                new Tag(640,"Price2","Price"),
                new Tag(641,"LastForwardPoints2","PriceOffset"),
                new Tag(642,"BidForwardPoints2","PriceOffset"),
                new Tag(643,"OfferForwardPoints2","PriceOffset"),
                new Tag(644,"RFQReqID","String"),
                new Tag(645,"MktBidPx","Price"),
                new Tag(646,"MktOfferPx","Price"),
                new Tag(647,"MinBidSize","Qty"),
                new Tag(648,"MinOfferSize","Qty"),
                new Tag(649,"QuoteStatusReqID","String"),
                new Tag(650,"LegalConfirm","Boolean"),
                new Tag(651,"UnderlyingLastPx","Price"),
                new Tag(652,"UnderlyingLastQty","Qty"),
                new Tag(653,"SecDefStatus","int"),
                new Tag(654,"LegRefID","String"),
                new Tag(655,"ContraLegRefID","String"),
                new Tag(656,"SettlCurrBidFxRate","float"),
                new Tag(657,"SettlCurrOfferFxRate","float"),
                new Tag(658,"QuoteRequestRejectReason","int"),
                new Tag(659,"SideComplianceID","String"),
                new Tag(660,"AcctIDSource","int"),
                new Tag(661,"AllocAcctIDSource","int"),
                new Tag(662,"BenchmarkPrice","Price"),
                new Tag(663,"BenchmarkPriceType","int"),
                new Tag(664,"ConfirmID","String"),
                new Tag(665,"ConfirmStatus","int"),
                new Tag(666,"ConfirmTransType","int"),
                new Tag(667,"ContractSettlMonth","MonthYear"),
                new Tag(668,"DeliveryForm","int"),
                new Tag(669,"LastParPx","Price"),
                new Tag(670,"NoLegAllocs","NumInGroup"),
                new Tag(671,"LegAllocAccount","String"),
                new Tag(672,"LegIndividualAllocID","String"),
                new Tag(673,"LegAllocQty","Qty"),
                new Tag(674,"LegAllocAcctIDSource","String"),
                new Tag(675,"LegSettlCurrency","Currency"),
                new Tag(676,"LegBenchmarkCurveCurrency","Currency"),
                new Tag(677,"LegBenchmarkCurveName","String"),
                new Tag(678,"LegBenchmarkCurvePoint","String"),
                new Tag(679,"LegBenchmarkPrice","Price"),
                new Tag(680,"LegBenchmarkPriceType","int"),
                new Tag(681,"LegBidPx","Price"),
                new Tag(682,"LegIOIQty","String"),
                new Tag(683,"NoLegStipulations","NumInGroup"),
                new Tag(684,"LegOfferPx","Price"),
                new Tag(685,"LegOrderQty","Qty"),
                new Tag(686,"LegPriceType","int"),
                new Tag(687,"LegQty","Qty"),
                new Tag(688,"LegStipulationType","String"),
                new Tag(689,"LegStipulationValue","String"),
                new Tag(690,"LegSwapType","int"),
                new Tag(691,"Pool","String"),
                new Tag(692,"QuotePriceType","int"),
                new Tag(693,"QuoteRespID","String"),
                new Tag(694,"QuoteRespType","int"),
                new Tag(695,"QuoteQualifier","char"),
                new Tag(696,"YieldRedemptionDate","LocalMktDate"),
                new Tag(697,"YieldRedemptionPrice","Price"),
                new Tag(698,"YieldRedemptionPriceType","int"),
                new Tag(699,"BenchmarkSecurityID","String"),
                new Tag(700,"ReversalIndicator","Boolean"),
                new Tag(701,"YieldCalcDate","LocalMktDate"),
                new Tag(702,"NoPositions","NumInGroup"),
                new Tag(703,"PosType","String"),
                new Tag(704,"LongQty","Qty"),
                new Tag(705,"ShortQty","Qty"),
                new Tag(706,"PosQtyStatus","int"),
                new Tag(707,"PosAmtType","String"),
                new Tag(708,"PosAmt","Amt"),
                new Tag(709,"PosTransType","int"),
                new Tag(710,"PosReqID","String"),
                new Tag(711,"NoUnderlyings","NumInGroup"),
                new Tag(712,"PosMaintAction","int"),
                new Tag(713,"OrigPosReqRefID","String"),
                new Tag(714,"PosMaintRptRefID","String"),
                new Tag(715,"ClearingBusinessDate","LocalMktDate"),
                new Tag(716,"SettlSessID","String"),
                new Tag(717,"SettlSessSubID","String"),
                new Tag(718,"AdjustmentType","int"),
                new Tag(719,"ContraryInstructionIndicator","Boolean"),
                new Tag(720,"PriorSpreadIndicator","Boolean"),
                new Tag(721,"PosMaintRptID","String"),
                new Tag(722,"PosMaintStatus","int"),
                new Tag(723,"PosMaintResult","int"),
                new Tag(724,"PosReqType","int"),
                new Tag(725,"ResponseTransportType","int"),
                new Tag(726,"ResponseDestination","String"),
                new Tag(727,"TotalNumPosReports","int"),
                new Tag(728,"PosReqResult","int"),
                new Tag(729,"PosReqStatus","int"),
                new Tag(730,"SettlPrice","Price"),
                new Tag(731,"SettlPriceType","int"),
                new Tag(732,"UnderlyingSettlPrice","Price"),
                new Tag(733,"UnderlyingSettlPriceType","int"),
                new Tag(734,"PriorSettlPrice","Price"),
                new Tag(735,"NoQuoteQualifiers","NumInGroup"),
                new Tag(736,"AllocSettlCurrency","Currency"),
                new Tag(737,"AllocSettlCurrAmt","Amt"),
                new Tag(738,"InterestAtMaturity","Amt"),
                new Tag(739,"LegDatedDate","LocalMktDate"),
                new Tag(740,"LegPool","String"),
                new Tag(741,"AllocInterestAtMaturity","Amt"),
                new Tag(742,"AllocAccruedInterestAmt","Amt"),
                new Tag(743,"DeliveryDate","LocalMktDate"),
                new Tag(744,"AssignmentMethod","char"),
                new Tag(745,"AssignmentUnit","Qty"),
                new Tag(746,"OpenInterest","Amt"),
                new Tag(747,"ExerciseMethod","char"),
                new Tag(748,"TotNumTradeReports","int"),
                new Tag(749,"TradeRequestResult","int"),
                new Tag(750,"TradeRequestStatus","int"),
                new Tag(751,"TradeReportRejectReason","int"),
                new Tag(752,"SideMultiLegReportingType","int"),
                new Tag(753,"NoPosAmt","NumInGroup"),
                new Tag(754,"AutoAcceptIndicator","Boolean"),
                new Tag(755,"AllocReportID","String"),
                new Tag(756,"NoNested2PartyIDs","NumInGroup"),
                new Tag(757,"Nested2PartyID","String"),
                new Tag(758,"Nested2PartyIDSource","char"),
                new Tag(759,"Nested2PartyRole","int"),
                new Tag(760,"Nested2PartySubID","String"),
                new Tag(761,"BenchmarkSecurityIDSource","String"),
                new Tag(762,"SecuritySubType","String"),
                new Tag(763,"UnderlyingSecuritySubType","String"),
                new Tag(764,"LegSecuritySubType","String"),
                new Tag(765,"AllowableOneSidednessPct","Percentage"),
                new Tag(766,"AllowableOneSidednessValue","Amt"),
                new Tag(767,"AllowableOneSidednessCurr","Currency"),
                new Tag(768,"NoTrdRegTimestamps","NumInGroup"),
                new Tag(769,"TrdRegTimestamp","UTCTimestamp"),
                new Tag(770,"TrdRegTimestampType","int"),
                new Tag(771,"TrdRegTimestampOrigin","String"),
                new Tag(772,"ConfirmRefID","String"),
                new Tag(773,"ConfirmType","int"),
                new Tag(774,"ConfirmRejReason","int"),
                new Tag(775,"BookingType","int"),
                new Tag(776,"IndividualAllocRejCode","int"),
                new Tag(777,"SettlInstMsgID","String"),
                new Tag(778,"NoSettlInst","NumInGroup"),
                new Tag(779,"LastUpdateTime","UTCTimestamp"),
                new Tag(780,"AllocSettlInstType","int"),
                new Tag(781,"NoSettlPartyIDs","NumInGroup"),
                new Tag(782,"SettlPartyID","String"),
                new Tag(783,"SettlPartyIDSource","char"),
                new Tag(784,"SettlPartyRole","int"),
                new Tag(785,"SettlPartySubID","String"),
                new Tag(786,"SettlPartySubIDType","int"),
                new Tag(787,"DlvyInstType","char"),
                new Tag(788,"TerminationType","int"),
                new Tag(789,"NextExpectedMsgSeqNum","SeqNum"),
                new Tag(790,"OrdStatusReqID","String"),
                new Tag(791,"SettlInstReqID","String"),
                new Tag(792,"SettlInstReqRejCode","int"),
                new Tag(793,"SecondaryAllocID","String"),
                new Tag(794,"AllocReportType","int"),
                new Tag(795,"AllocReportRefID","String"),
                new Tag(796,"AllocCancReplaceReason","int"),
                new Tag(797,"CopyMsgIndicator","Boolean"),
                new Tag(798,"AllocAccountType","int"),
                new Tag(799,"OrderAvgPx","Price"),
                new Tag(800,"OrderBookingQty","Qty"),
                new Tag(801,"NoSettlPartySubIDs","NumInGroup"),
                new Tag(802,"NoPartySubIDs","NumInGroup"),
                new Tag(803,"PartySubIDType","int"),
                new Tag(804,"NoNestedPartySubIDs","NumInGroup"),
                new Tag(805,"NestedPartySubIDType","int"),
                new Tag(806,"NoNested2PartySubIDs","NumInGroup"),
                new Tag(807,"Nested2PartySubIDType","int"),
                new Tag(808,"AllocIntermedReqType","int"),
                new Tag(809,"NoUsernames","NumInGroup"),
                new Tag(810,"UnderlyingPx","Price"),
                new Tag(811,"PriceDelta","float"),
                new Tag(812,"ApplQueueMax","int"),
                new Tag(813,"ApplQueueDepth","int"),
                new Tag(814,"ApplQueueResolution","int"),
                new Tag(815,"ApplQueueAction","int"),
                new Tag(816,"NoAltMDSource","NumInGroup"),
                new Tag(817,"AltMDSourceID","String"),
                new Tag(818,"SecondaryTradeReportID","String"),
                new Tag(819,"AvgPxIndicator","int"),
                new Tag(820,"TradeLinkID","String"),
                new Tag(821,"OrderInputDevice","String"),
                new Tag(822,"UnderlyingTradingSessionID","String"),
                new Tag(823,"UnderlyingTradingSessionSubID","String"),
                new Tag(824,"TradeLegRefID","String"),
                new Tag(825,"ExchangeRule","String"),
                new Tag(826,"TradeAllocIndicator","int"),
                new Tag(827,"ExpirationCycle","int"),
                new Tag(828,"TrdType","int"),
                new Tag(829,"TrdSubType","int"),
                new Tag(830,"TransferReason","String"),
                new Tag(831,"AsgnReqID","String"),
                new Tag(832,"TotNumAssignmentReports","int"),
                new Tag(833,"AsgnRptID","String"),
                new Tag(834,"ThresholdAmount","PriceOffset"),
                new Tag(835,"PegMoveType","int"),
                new Tag(836,"PegOffsetType","int"),
                new Tag(837,"PegLimitType","int"),
                new Tag(838,"PegRoundDirection","int"),
                new Tag(839,"PeggedPrice","Price"),
                new Tag(840,"PegScope","int"),
                new Tag(841,"DiscretionMoveType","int"),
                new Tag(842,"DiscretionOffsetType","int"),
                new Tag(843,"DiscretionLimitType","int"),
                new Tag(844,"DiscretionRoundDirection","int"),
                new Tag(845,"DiscretionPrice","Price"),
                new Tag(846,"DiscretionScope","int"),
                new Tag(847,"TargetStrategy","int"),
                new Tag(848,"TargetStrategyParameters","String"),
                new Tag(849,"ParticipationRate","Percentage"),
                new Tag(850,"TargetStrategyPerformance","float"),
                new Tag(851,"LastLiquidityInd","int"),
                new Tag(852,"PublishTrdIndicator","Boolean"),
                new Tag(853,"ShortSaleReason","int"),
                new Tag(854,"QtyType","int"),
                new Tag(855,"SecondaryTrdType","int"),
                new Tag(856,"TradeReportType","int"),
                new Tag(857,"AllocNoOrdersType","int"),
                new Tag(858,"SharedCommission","Amt"),
                new Tag(859,"ConfirmReqID","String"),
                new Tag(860,"AvgParPx","Price"),
                new Tag(861,"ReportedPx","Price"),
                new Tag(862,"NoCapacities","NumInGroup"),
                new Tag(863,"OrderCapacityQty","Qty"),
                new Tag(864,"NoEvents","NumInGroup"),
                new Tag(865,"EventType","int"),
                new Tag(866,"EventDate","LocalMktDate"),
                new Tag(867,"EventPx","Price"),
                new Tag(868,"EventText","String"),
                new Tag(869,"PctAtRisk","Percentage"),
                new Tag(870,"NoInstrAttrib","NumInGroup"),
                new Tag(871,"InstrAttribType","int"),
                new Tag(872,"InstrAttribValue","String"),
                new Tag(873,"DatedDate","LocalMktDate"),
                new Tag(874,"InterestAccrualDate","LocalMktDate"),
                new Tag(875,"CPProgram","int"),
                new Tag(876,"CPRegType","String"),
                new Tag(877,"UnderlyingCPProgram","String"),
                new Tag(878,"UnderlyingCPRegType","String"),
                new Tag(879,"UnderlyingQty","Qty"),
                new Tag(880,"TrdMatchID","String"),
                new Tag(881,"SecondaryTradeReportRefID","String"),
                new Tag(882,"UnderlyingDirtyPrice","Price"),
                new Tag(883,"UnderlyingEndPrice","Price"),
                new Tag(884,"UnderlyingStartValue","Amt"),
                new Tag(885,"UnderlyingCurrentValue","Amt"),
                new Tag(886,"UnderlyingEndValue","Amt"),
                new Tag(887,"NoUnderlyingStips","NumInGroup"),
                new Tag(888,"UnderlyingStipType","String"),
                new Tag(889,"UnderlyingStipValue","String"),
                new Tag(890,"MaturityNetMoney","Amt"),
                new Tag(891,"MiscFeeBasis","int"),
                new Tag(892,"TotNoAllocs","int"),
                new Tag(893,"LastFragment","Boolean"),
                new Tag(894,"CollReqID","String"),
                new Tag(895,"CollAsgnReason","int"),
                new Tag(896,"CollInquiryQualifier","int"),
                new Tag(897,"NoTrades","NumInGroup"),
                new Tag(898,"MarginRatio","Percentage"),
                new Tag(899,"MarginExcess","Amt"),
                new Tag(900,"TotalNetValue","Amt"),
                new Tag(901,"CashOutstanding","Amt"),
                new Tag(902,"CollAsgnID","String"),
                new Tag(903,"CollAsgnTransType","int"),
                new Tag(904,"CollRespID","String"),
                new Tag(905,"CollAsgnRespType","int"),
                new Tag(906,"CollAsgnRejectReason","int"),
                new Tag(907,"CollAsgnRefID","String"),
                new Tag(908,"CollRptID","String"),
                new Tag(909,"CollInquiryID","String"),
                new Tag(910,"CollStatus","int"),
                new Tag(911,"TotNumReports","int"),
                new Tag(912,"LastRptRequested","Boolean"),
                new Tag(913,"AgreementDesc","String"),
                new Tag(914,"AgreementID","String"),
                new Tag(915,"AgreementDate","LocalMktDate"),
                new Tag(916,"StartDate","LocalMktDate"),
                new Tag(917,"EndDate","LocalMktDate"),
                new Tag(918,"AgreementCurrency","Currency"),
                new Tag(919,"DeliveryType","int"),
                new Tag(920,"EndAccruedInterestAmt","Amt"),
                new Tag(921,"StartCash","Amt"),
                new Tag(922,"EndCash","Amt"),
                new Tag(923,"UserRequestID","String"),
                new Tag(924,"UserRequestType","int"),
                new Tag(925,"NewPassword","String"),
                new Tag(926,"UserStatus","int"),
                new Tag(927,"UserStatusText","String"),
                new Tag(928,"StatusValue","int"),
                new Tag(929,"StatusText","String"),
                new Tag(930,"RefCompID","String"),
                new Tag(931,"RefSubID","String"),
                new Tag(932,"NetworkResponseID","String"),
                new Tag(933,"NetworkRequestID","String"),
                new Tag(934,"LastNetworkResponseID","String"),
                new Tag(935,"NetworkRequestType","int"),
                new Tag(936,"NoCompIDs","NumInGroup"),
                new Tag(937,"NetworkStatusResponseType","int"),
                new Tag(938,"NoCollInquiryQualifier","NumInGroup"),
                new Tag(939,"TrdRptStatus","int"),
                new Tag(940,"AffirmStatus","int"),
                new Tag(941,"UnderlyingStrikeCurrency","Currency"),
                new Tag(942,"LegStrikeCurrency","Currency"),
                new Tag(943,"TimeBracket","String"),
                new Tag(944,"CollAction","int"),
                new Tag(945,"CollInquiryStatus","int"),
                new Tag(946,"CollInquiryResult","int"),
                new Tag(947,"StrikeCurrency","Currency"),
                new Tag(948,"NoNested3PartyIDs","NumInGroup"),
                new Tag(949,"Nested3PartyID","String"),
                new Tag(950,"Nested3PartyIDSource","char"),
                new Tag(951,"Nested3PartyRole","int"),
                new Tag(952,"NoNested3PartySubIDs","NumInGroup"),
                new Tag(953,"Nested3PartySubID","String"),
                new Tag(954,"Nested3PartySubIDType","int"),
                new Tag(955,"LegContractSettlMonth","MonthYear"),
                new Tag(956,"LegInterestAccrualDate","LocalMktDate"),
                new Tag(957,"NoStrategyParameters","NumInGroup"),
                new Tag(958,"StrategyParameterName","String"),
                new Tag(959,"StrategyParameterType","int"),
                new Tag(960,"StrategyParameterValue","String"),
                new Tag(961,"HostCrossID","String"),
                new Tag(962,"SideTimeInForce","UTCTimestamp"),
                new Tag(963,"MDReportID","int"),
                new Tag(964,"SecurityReportID","int"),
                new Tag(965,"SecurityStatus","String"),
                new Tag(966,"SettleOnOpenFlag","String"),
                new Tag(967,"StrikeMultiplier","float"),
                new Tag(968,"StrikeValue","float"),
                new Tag(969,"MinPriceIncrement","float"),
                new Tag(970,"PositionLimit","int"),
                new Tag(971,"NTPositionLimit","int"),
                new Tag(972,"UnderlyingAllocationPercent","Percentage"),
                new Tag(973,"UnderlyingCashAmount","Amt"),
                new Tag(974,"UnderlyingCashType","String"),
                new Tag(975,"UnderlyingSettlementType","int"),
                new Tag(976,"QuantityDate","LocalMktDate"),
                new Tag(977,"ContIntRptID","String"),
                new Tag(978,"LateIndicator","Boolean"),
                new Tag(979,"InputSource","String"),
                new Tag(980,"SecurityUpdateAction","char"),
                new Tag(981,"NoExpiration","NumInGroup"),
                new Tag(982,"ExpirationQtyType","int"),
                new Tag(983,"ExpQty","Qty"),
                new Tag(984,"NoUnderlyingAmounts","NumInGroup"),
                new Tag(985,"UnderlyingPayAmount","Amt"),
                new Tag(986,"UnderlyingCollectAmount","Amt"),
                new Tag(987,"UnderlyingSettlementDate","LocalMktDate"),
                new Tag(988,"UnderlyingSettlementStatus","String"),
                new Tag(989,"SecondaryIndividualAllocID","String"),
                new Tag(990,"LegReportID","String"),
                new Tag(991,"RndPx","Price"),
                new Tag(992,"IndividualAllocType","int"),
                new Tag(993,"AllocCustomerCapacity","String"),
                new Tag(994,"TierCode","String"),
                new Tag(996,"UnitOfMeasure","String"),
                new Tag(997,"TimeUnit","String"),
                new Tag(998,"UnderlyingUnitOfMeasure","String"),
                new Tag(999,"LegUnitOfMeasure","String"),
                new Tag(1000,"UnderlyingTimeUnit","String"),
                new Tag(1001,"LegTimeUnit","String"),
                new Tag(1002,"AllocMethod","int"),
                new Tag(1003,"TradeID","String"),
                new Tag(1005,"SideTradeReportID","String"),
                new Tag(1006,"SideFillStationCd","String"),
                new Tag(1007,"SideReasonCd","String"),
                new Tag(1008,"SideTrdSubTyp","int"),
                new Tag(1009,"SideLastQty","int"),
                new Tag(1011,"MessageEventSource","String"),
                new Tag(1012,"SideTrdRegTimestamp","UTCTimestamp"),
                new Tag(1013,"SideTrdRegTimestampType","int"),
                new Tag(1014,"SideTrdRegTimestampSrc","String"),
                new Tag(1015,"AsOfIndicator","char"),
                new Tag(1016,"NoSideTrdRegTS","NumInGroup"),
                new Tag(1017,"LegOptionRatio","float"),
                new Tag(1018,"NoInstrumentParties","NumInGroup"),
                new Tag(1019,"InstrumentPartyID","String"),
                new Tag(1020,"TradeVolume","Qty"),
                new Tag(1021,"MDBookType","int"),
                new Tag(1022,"MDFeedType","String"),
                new Tag(1023,"MDPriceLevel","int"),
                new Tag(1024,"MDOriginType","int"),
                new Tag(1025,"FirstPx","Price"),
                new Tag(1026,"MDEntrySpotRate","float"),
                new Tag(1027,"MDEntryForwardPoints","PriceOffset"),
                new Tag(1028,"ManualOrderIndicator","Boolean"),
                new Tag(1029,"CustDirectedOrder","Boolean"),
                new Tag(1030,"ReceivedDeptID","String"),
                new Tag(1031,"CustOrderHandlingInst","MultipleStringValue"),
                new Tag(1032,"OrderHandlingInstSource","int"),
                new Tag(1033,"DeskType","String"),
                new Tag(1034,"DeskTypeSource","int"),
                new Tag(1035,"DeskOrderHandlingInst","MultipleStringValue"),
                new Tag(1036,"ExecAckStatus","char"),
                new Tag(1037,"UnderlyingDeliveryAmount","Amt"),
                new Tag(1038,"UnderlyingCapValue","Amt"),
                new Tag(1039,"UnderlyingSettlMethod","String"),
                new Tag(1040,"SecondaryTradeID","String"),
                new Tag(1041,"FirmTradeID","String"),
                new Tag(1042,"SecondaryFirmTradeID","String"),
                new Tag(1043,"CollApplType","int"),
                new Tag(1044,"UnderlyingAdjustedQuantity","Qty"),
                new Tag(1045,"UnderlyingFXRate","float"),
                new Tag(1046,"UnderlyingFXRateCalc","char"),
                new Tag(1047,"AllocPositionEffect","char"),
                new Tag(1048,"DealingCapacity","char"),
                new Tag(1049,"InstrmtAssignmentMethod","char"),
                new Tag(1050,"InstrumentPartyIDSource","char"),
                new Tag(1051,"InstrumentPartyRole","int"),
                new Tag(1052,"NoInstrumentPartySubIDs","NumInGroup"),
                new Tag(1053,"InstrumentPartySubID","String"),
                new Tag(1054,"InstrumentPartySubIDType","int"),
                new Tag(1055,"PositionCurrency","String"),
                new Tag(1056,"CalculatedCcyLastQty","Qty"),
                new Tag(1057,"AggressorIndicator","Boolean"),
                new Tag(1058,"NoUndlyInstrumentParties","NumInGroup"),
                new Tag(1059,"UnderlyingInstrumentPartyID","String"),
                new Tag(1060,"UnderlyingInstrumentPartyIDSource","char"),
                new Tag(1061,"UnderlyingInstrumentPartyRole","int"),
                new Tag(1062,"NoUndlyInstrumentPartySubIDs","NumInGroup"),
                new Tag(1063,"UnderlyingInstrumentPartySubID","String"),
                new Tag(1064,"UnderlyingInstrumentPartySubIDType","int"),
                new Tag(1065,"BidSwapPoints","PriceOffset"),
                new Tag(1066,"OfferSwapPoints","PriceOffset"),
                new Tag(1067,"LegBidForwardPoints","PriceOffset"),
                new Tag(1068,"LegOfferForwardPoints","PriceOffset"),
                new Tag(1069,"SwapPoints","PriceOffset"),
                new Tag(1070,"MDQuoteType","int"),
                new Tag(1071,"LastSwapPoints","PriceOffset"),
                new Tag(1072,"SideGrossTradeAmt","Amt"),
                new Tag(1073,"LegLastForwardPoints","PriceOffset"),
                new Tag(1074,"LegCalculatedCcyLastQty","Qty"),
                new Tag(1075,"LegGrossTradeAmt","Amt"),
                new Tag(1079,"MaturityTime","TZTimeOnly"),
                new Tag(1080,"RefOrderID","String"),
                new Tag(1081,"RefOrderIDSource","char"),
                new Tag(1082,"SecondaryDisplayQty","Qty"),
                new Tag(1083,"DisplayWhen","char"),
                new Tag(1084,"DisplayMethod","char"),
                new Tag(1085,"DisplayLowQty","Qty"),
                new Tag(1086,"DisplayHighQty","Qty"),
                new Tag(1087,"DisplayMinIncr","Qty"),
                new Tag(1088,"RefreshQty","Qty"),
                new Tag(1089,"MatchIncrement","Qty"),
                new Tag(1090,"MaxPriceLevels","int"),
                new Tag(1091,"PreTradeAnonymity","Boolean"),
                new Tag(1092,"PriceProtectionScope","char"),
                new Tag(1093,"LotType","char"),
                new Tag(1094,"PegPriceType","int"),
                new Tag(1095,"PeggedRefPrice","Price"),
                new Tag(1096,"PegSecurityIDSource","String"),
                new Tag(1097,"PegSecurityID","String"),
                new Tag(1098,"PegSymbol","String"),
                new Tag(1099,"PegSecurityDesc","String"),
                new Tag(1100,"TriggerType","char"),
                new Tag(1101,"TriggerAction","char"),
                new Tag(1102,"TriggerPrice","Price"),
                new Tag(1103,"TriggerSymbol","String"),
                new Tag(1104,"TriggerSecurityID","String"),
                new Tag(1105,"TriggerSecurityIDSource","String"),
                new Tag(1106,"TriggerSecurityDesc","String"),
                new Tag(1107,"TriggerPriceType","char"),
                new Tag(1108,"TriggerPriceTypeScope","char"),
                new Tag(1109,"TriggerPriceDirection","char"),
                new Tag(1110,"TriggerNewPrice","Price"),
                new Tag(1111,"TriggerOrderType","char"),
                new Tag(1112,"TriggerNewQty","Qty"),
                new Tag(1113,"TriggerTradingSessionID","String"),
                new Tag(1114,"TriggerTradingSessionSubID","String"),
                new Tag(1115,"OrderCategory","char"),
                new Tag(1116,"NoRootPartyIDs","NumInGroup"),
                new Tag(1117,"RootPartyID","String"),
                new Tag(1118,"RootPartyIDSource","char"),
                new Tag(1119,"RootPartyRole","int"),
                new Tag(1120,"NoRootPartySubIDs","NumInGroup"),
                new Tag(1121,"RootPartySubID","String"),
                new Tag(1122,"RootPartySubIDType","int"),
                new Tag(1123,"TradeHandlingInstr","char"),
                new Tag(1124,"OrigTradeHandlingInstr","char"),
                new Tag(1125,"OrigTradeDate","LocalMktDate"),
                new Tag(1126,"OrigTradeID","String"),
                new Tag(1127,"OrigSecondaryTradeID","String"),
                new Tag(1128,"ApplVerID","String"),
                new Tag(1129,"CstmApplVerID","String"),
                new Tag(1130,"RefApplVerID","String"),
                new Tag(1131,"RefCstmApplVerID","String"),
                new Tag(1132,"TZTransactTime","TZTimestamp"),
                new Tag(1133,"ExDestinationIDSource","char"),
                new Tag(1134,"ReportedPxDiff","Boolean"),
                new Tag(1135,"RptSys","String"),
                new Tag(1136,"AllocClearingFeeIndicator","String"),
                new Tag(1137,"DefaultApplVerID","String"),
                new Tag(1138,"DisplayQty","Qty"),
                new Tag(1139,"ExchangeSpecialInstructions","String"),
                new Tag(1140,"MaxTradeVol","Qty"),
                new Tag(1141,"NoMDFeedTypes","NumInGroup"),
                new Tag(1142,"MatchAlgorithm","String"),
                new Tag(1143,"MaxPriceVariation","float"),
                new Tag(1144,"ImpliedMarketIndicator","int"),
                new Tag(1145,"EventTime","UTCTimestamp"),
                new Tag(1146,"MinPriceIncrementAmount","Amt"),
                new Tag(1147,"UnitOfMeasureQty","Qty"),
                new Tag(1148,"LowLimitPrice","Price"),
                new Tag(1149,"HighLimitPrice","Price"),
                new Tag(1150,"TradingReferencePrice","Price"),
                new Tag(1151,"SecurityGroup","String"),
                new Tag(1152,"LegNumber","int"),
                new Tag(1153,"SettlementCycleNo","int"),
                new Tag(1154,"SideCurrency","Currency"),
                new Tag(1155,"SideSettlCurrency","Currency"),
                new Tag(1156,"ApplExtID","int"),
                new Tag(1157,"CcyAmt","Amt"),
                new Tag(1158,"NoSettlDetails","NumInGroup"),
                new Tag(1159,"SettlObligMode","int"),
                new Tag(1160,"SettlObligMsgID","String"),
                new Tag(1161,"SettlObligID","String"),
                new Tag(1162,"SettlObligTransType","char"),
                new Tag(1163,"SettlObligRefID","String"),
                new Tag(1164,"SettlObligSource","char"),
                new Tag(1165,"NoSettlOblig","NumInGroup"),
                new Tag(1166,"QuoteMsgID","String"),
                new Tag(1167,"QuoteEntryStatus","int"),
                new Tag(1168,"TotNoCxldQuotes","int"),
                new Tag(1169,"TotNoAccQuotes","int"),
                new Tag(1170,"TotNoRejQuotes","int"),
                new Tag(1171,"PrivateQuote","Boolean"),
                new Tag(1172,"RespondentType","int"),
                new Tag(1173,"MDSubBookType","int"),
                new Tag(1174,"SecurityTradingEvent","int"),
                new Tag(1175,"NoStatsIndicators","NumInGroup"),
                new Tag(1176,"StatsType","int"),
                new Tag(1177,"NoOfSecSizes","NumInGroup"),
                new Tag(1178,"MDSecSizeType","int"),
                new Tag(1179,"MDSecSize","Qty"),
                new Tag(1180,"ApplID","String"),
                new Tag(1181,"ApplSeqNum","SeqNum"),
                new Tag(1182,"ApplBegSeqNum","SeqNum"),
                new Tag(1183,"ApplEndSeqNum","SeqNum"),
                new Tag(1184,"SecurityXMLLen","Length"),
                new Tag(1185,"SecurityXML","XMLData"),
                new Tag(1186,"SecurityXMLSchema","String"),
                new Tag(1187,"RefreshIndicator","Boolean"),
                new Tag(1188,"Volatility","float"),
                new Tag(1189,"TimeToExpiration","float"),
                new Tag(1190,"RiskFreeRate","float"),
                new Tag(1191,"PriceUnitOfMeasure","String"),
                new Tag(1192,"PriceUnitOfMeasureQty","Qty"),
                new Tag(1193,"SettlMethod","char"),
                new Tag(1194,"ExerciseStyle","int"),
                new Tag(1195,"OptPayoutAmount","Amt"),
                new Tag(1196,"PriceQuoteMethod","String"),
                new Tag(1197,"ValuationMethod","String"),
                new Tag(1198,"ListMethod","int"),
                new Tag(1199,"CapPrice","Price"),
                new Tag(1200,"FloorPrice","Price"),
                new Tag(1201,"NoStrikeRules","NumInGroup"),
                new Tag(1202,"StartStrikePxRange","Price"),
                new Tag(1203,"EndStrikePxRange","Price"),
                new Tag(1204,"StrikeIncrement","float"),
                new Tag(1205,"NoTickRules","NumInGroup"),
                new Tag(1206,"StartTickPriceRange","Price"),
                new Tag(1207,"EndTickPriceRange","Price"),
                new Tag(1208,"TickIncrement","Price"),
                new Tag(1209,"TickRuleType","int"),
                new Tag(1210,"NestedInstrAttribType","int"),
                new Tag(1211,"NestedInstrAttribValue","String"),
                new Tag(1212,"LegMaturityTime","TZTimeOnly"),
                new Tag(1213,"UnderlyingMaturityTime","TZTimeOnly"),
                new Tag(1214,"DerivativeSymbol","String"),
                new Tag(1215,"DerivativeSymbolSfx","String"),
                new Tag(1216,"DerivativeSecurityID","String"),
                new Tag(1217,"DerivativeSecurityIDSource","String"),
                new Tag(1218,"NoDerivativeSecurityAltID","NumInGroup"),
                new Tag(1219,"DerivativeSecurityAltID","String"),
                new Tag(1220,"DerivativeSecurityAltIDSource","String"),
                new Tag(1221,"SecondaryLowLimitPrice","Price"),
                new Tag(1222,"MaturityRuleID","String"),
                new Tag(1223,"StrikeRuleID","String"),
                new Tag(1224,"LegUnitOfMeasureQty","Qty"),
                new Tag(1225,"DerivativeOptPayAmount","Amt"),
                new Tag(1226,"EndMaturityMonthYear","MonthYear"),
                new Tag(1227,"ProductComplex","String"),
                new Tag(1228,"DerivativeProductComplex","String"),
                new Tag(1229,"MaturityMonthYearIncrement","int"),
                new Tag(1230,"SecondaryHighLimitPrice","Price"),
                new Tag(1231,"MinLotSize","Qty"),
                new Tag(1232,"NoExecInstRules","NumInGroup"),
                new Tag(1234,"NoLotTypeRules","NumInGroup"),
                new Tag(1235,"NoMatchRules","NumInGroup"),
                new Tag(1236,"NoMaturityRules","NumInGroup"),
                new Tag(1237,"NoOrdTypeRules","NumInGroup"),
                new Tag(1239,"NoTimeInForceRules","NumInGroup"),
                new Tag(1240,"SecondaryTradingReferencePrice","Price"),
                new Tag(1241,"StartMaturityMonthYear","MonthYear"),
                new Tag(1242,"FlexProductEligibilityIndicator","Boolean"),
                new Tag(1243,"DerivFlexProductEligibilityIndicator","Boolean"),
                new Tag(1244,"FlexibleIndicator","Boolean"),
                new Tag(1245,"TradingCurrency","Currency"),
                new Tag(1246,"DerivativeProduct","int"),
                new Tag(1247,"DerivativeSecurityGroup","String"),
                new Tag(1248,"DerivativeCFICode","String"),
                new Tag(1249,"DerivativeSecurityType","String"),
                new Tag(1250,"DerivativeSecuritySubType","String"),
                new Tag(1251,"DerivativeMaturityMonthYear","MonthYear"),
                new Tag(1252,"DerivativeMaturityDate","LocalMktDate"),
                new Tag(1253,"DerivativeMaturityTime","TZTimeOnly"),
                new Tag(1254,"DerivativeSettleOnOpenFlag","String"),
                new Tag(1255,"DerivativeInstrmtAssignmentMethod","char"),
                new Tag(1256,"DerivativeSecurityStatus","String"),
                new Tag(1257,"DerivativeInstrRegistry","String"),
                new Tag(1258,"DerivativeCountryOfIssue","Country"),
                new Tag(1259,"DerivativeStateOrProvinceOfIssue","String"),
                new Tag(1260,"DerivativeLocaleOfIssue","String"),
                new Tag(1261,"DerivativeStrikePrice","Price"),
                new Tag(1262,"DerivativeStrikeCurrency","Currency"),
                new Tag(1263,"DerivativeStrikeMultiplier","float"),
                new Tag(1264,"DerivativeStrikeValue","float"),
                new Tag(1265,"DerivativeOptAttribute","char"),
                new Tag(1266,"DerivativeContractMultiplier","float"),
                new Tag(1267,"DerivativeMinPriceIncrement","float"),
                new Tag(1268,"DerivativeMinPriceIncrementAmount","Amt"),
                new Tag(1269,"DerivativeUnitOfMeasure","String"),
                new Tag(1270,"DerivativeUnitOfMeasureQty","Qty"),
                new Tag(1271,"DerivativeTimeUnit","String"),
                new Tag(1272,"DerivativeSecurityExchange","Exchange"),
                new Tag(1456,"UnderlyingOriginalNotionalPercentageOutstanding","Percentage"),
                new Tag(1457,"AttachmentPoint","Percentage"),
                new Tag(1458,"DetachmentPoint","Percentage"),
                new Tag(1459,"UnderlyingAttachmentPoint","Percentage"),
                new Tag(1460,"UnderlyingDetachmentPoint","Percentage"),
                new Tag(1461,"NoTargetPartyIDs","NumInGroup"),
                new Tag(1462,"TargetPartyID","String"),
                new Tag(1463,"TargetPartyIDSource","char"),
                new Tag(1464,"TargetPartyRole","int"),
                new Tag(1465,"SecurityListID","String"),
                new Tag(1466,"SecurityListRefID","String"),
                new Tag(1467,"SecurityListDesc","String"),
                new Tag(1468,"EncodedSecurityListDescLen","Length"),
                new Tag(1469,"EncodedSecurityListDesc","data"),
                new Tag(1470,"SecurityListType","int"),
                new Tag(1471,"SecurityListTypeSource","int"),
                new Tag(1472,"NewsID","String"),
                new Tag(1473,"NewsCategory","int"),
                new Tag(1474,"LanguageCode","Language"),
                new Tag(1475,"NoNewsRefIDs","NumInGroup"),
                new Tag(1476,"NewsRefID","String"),
                new Tag(1477,"NewsRefType","int"),
                new Tag(1478,"StrikePriceDeterminationMethod","int"),
                new Tag(1479,"StrikePriceBoundaryMethod","int"),
                new Tag(1480,"StrikePriceBoundaryPrecision","Percentage"),
                new Tag(1481,"UnderlyingPriceDeterminationMethod","int"),
                new Tag(1482,"OptPayoutType","int"),
                new Tag(1483,"NoComplexEvents","NumInGroup"),
                new Tag(1484,"ComplexEventType","int"),
                new Tag(1485,"ComplexOptPayoutAmount","Amt"),
                new Tag(1486,"ComplexEventPrice","Price"),
                new Tag(1487,"ComplexEventPriceBoundaryMethod","int"),
                new Tag(1488,"ComplexEventPriceBoundaryPrecision","Percentage"),
                new Tag(1489,"ComplexEventPriceTimeType","int"),
                new Tag(1490,"ComplexEventCondition","int"),
                new Tag(1491,"NoComplexEventDates","NumInGroup"),
                new Tag(1492,"ComplexEventStartDate","UTCTimestamp"),
                new Tag(1493,"ComplexEventEndDate","UTCTimestamp"),
                new Tag(1494,"NoComplexEventTimes","NumInGroup"),
                new Tag(1495,"ComplexEventStartTime","UTCTimeOnly"),
                new Tag(1496,"ComplexEventEndTime","UTCTimeOnly"),
                new Tag(1497,"StreamAsgnReqID","String"),
                new Tag(1498,"StreamAsgnReqType","int"),
                new Tag(1499,"NoAsgnReqs","NumInGroup"),
                new Tag(1500,"MDStreamID","String"),
                new Tag(1501,"StreamAsgnRptID","String"),
                new Tag(1502,"StreamAsgnRejReason","int"),
                new Tag(1503,"StreamAsgnAckType","int"),
                new Tag(1504,"RelSymTransactTime","UTCTimestamp"),
                new Tag(1505,"PartyDetailsListRequestID","String"),
                new Tag(1506,"NoPartyListResponseTypes","NumInGroup"),
                new Tag(1507,"PartyListResponseType","int"),
                new Tag(1508,"NoRequestedPartyRoles","NumInGroup"),
                new Tag(1509,"RequestedPartyRole","int"),
                new Tag(1510,"PartyDetailsListReportID","String"),
                new Tag(1511,"PartyDetailsRequestResult","int"),
                new Tag(1512,"TotNoPartyList","int"),
                new Tag(1513,"NoPartyList","NumInGroup"),
                new Tag(1514,"NoPartyRelationships","NumInGroup"),
                new Tag(1515,"PartyRelationship","int"),
                new Tag(1516,"NoPartyAltIDs","NumInGroup"),
                new Tag(1517,"PartyAltID","String"),
                new Tag(1518,"PartyAltIDSource","char"),
                new Tag(1519,"NoPartyAltSubIDs","NumInGroup"),
                new Tag(1520,"PartyAltSubID","String"),
                new Tag(1521,"PartyAltSubIDType","int"),
                new Tag(1522,"NoContextPartyIDs","NumInGroup"),
                new Tag(1523,"ContextPartyID","String"),
                new Tag(1524,"ContextPartyIDSource","char"),
                new Tag(1525,"ContextPartyRole","int"),
                new Tag(1526,"NoContextPartySubIDs","NumInGroup"),
                new Tag(1527,"ContextPartySubID","String"),
                new Tag(1528,"ContextPartySubIDType","int"),
                new Tag(1529,"NoRiskLimits","NumInGroup"),
                new Tag(1530,"RiskLimitType","int"),
                new Tag(1531,"RiskLimitAmount","Amt"),
                new Tag(1532,"RiskLimitCurrency","Currency"),
                new Tag(1533,"RiskLimitPlatform","String"),
                new Tag(1534,"NoRiskInstruments","NumInGroup"),
                new Tag(1535,"RiskInstrumentOperator","int"),
                new Tag(1536,"RiskSymbol","String"),
                new Tag(1537,"RiskSymbolSfx","String"),
                new Tag(1538,"RiskSecurityID","String"),
                new Tag(1539,"RiskSecurityIDSource","String"),
                new Tag(1540,"NoRiskSecurityAltID","NumInGrp"),
                new Tag(1541,"RiskSecurityAltID","String"),
                new Tag(1542,"RiskSecurityAltIDSource","String"),
                new Tag(1543,"RiskProduct","int"),
                new Tag(1544,"RiskProductComplex","String"),
                new Tag(1545,"RiskSecurityGroup","String"),
                new Tag(1546,"RiskCFICode","String"),
                new Tag(1547,"RiskSecurityType","String"),
                new Tag(1548,"RiskSecuritySubType","String"),
                new Tag(1549,"RiskMaturityMonthYear","MonthYear"),
                new Tag(1550,"RiskMaturityTime","TZTimeOnly"),
                new Tag(1551,"RiskRestructuringType","String"),
                new Tag(1552,"RiskSeniority","String"),
                new Tag(1553,"RiskPutOrCall","int"),
                new Tag(1554,"RiskFlexibleIndicator","Boolean"),
                new Tag(1555,"RiskCouponRate","Percentage"),
                new Tag(1556,"RiskSecurityDesc","String"),
                new Tag(1557,"RiskInstrumentSettlType","String"),
                new Tag(1558,"RiskInstrumentMultiplier","float"),
                new Tag(1559,"NoRiskWarningLevels","NumInGroup"),
                new Tag(1560,"RiskWarningLevelPercent","Percentage"),
                new Tag(1561,"RiskWarningLevelName","String"),
                new Tag(1562,"NoRelatedPartyIDs","NumInGroup"),
                new Tag(1563,"RelatedPartyID","String"),
                new Tag(1564,"RelatedPartyIDSource","char"),
                new Tag(1565,"RelatedPartyRole","int"),
                new Tag(1566,"NoRelatedPartySubIDs","NumInGroup"),
                new Tag(1567,"RelatedPartySubID","String"),
                new Tag(1568,"RelatedPartySubIDType","int"),
                new Tag(1569,"NoRelatedPartyAltIDs","NumInGroup"),
                new Tag(1570,"RelatedPartyAltID","String"),
                new Tag(1571,"RelatedPartyAltIDSource","char"),
                new Tag(1572,"NoRelatedPartyAltSubIDs","NumInGroup"),
                new Tag(1573,"RelatedPartyAltSubID","String"),
                new Tag(1574,"RelatedPartyAltSubIDType","int"),
                new Tag(1575,"NoRelatedContextPartyIDs","NumInGroup"),
                new Tag(1576,"RelatedContextPartyID","String"),
                new Tag(1577,"RelatedContextPartyIDSource","char"),
                new Tag(1578,"RelatedContextPartyRole","int"),
                new Tag(1579,"NoRelatedContextPartySubIDs","NumInGroup"),
                new Tag(1580,"RelatedContextPartySubID","String"),
                new Tag(1581,"RelatedContextPartySubIDType","int"),
                new Tag(1582,"NoRelationshipRiskLimits","NumInGroup"),
                new Tag(1583,"RelationshipRiskLimitType","int"),
                new Tag(1584,"RelationshipRiskLimitAmount","Amt"),
                new Tag(1585,"RelationshipRiskLimitCurrency","Currency"),
                new Tag(1586,"RelationshipRiskLimitPlatform","String"),
                new Tag(1587,"NoRelationshipRiskInstruments","NumInGroup"),
                new Tag(1588,"RelationshipRiskInstrumentOperator","int"),
                new Tag(1589,"RelationshipRiskSymbol","String"),
                new Tag(1590,"RelationshipRiskSymbolSfx","String"),
                new Tag(1591,"RelationshipRiskSecurityID","String"),
                new Tag(1592,"RelationshipRiskSecurityIDSource","String"),
                new Tag(1593,"NoRelationshipRiskSecurityAltID","NumInGrp"),
                new Tag(1594,"RelationshipRiskSecurityAltID","String"),
                new Tag(1595,"RelationshipRiskSecurityAltIDSource","String"),
                new Tag(1596,"RelationshipRiskProduct","int"),
                new Tag(1597,"RelationshipRiskProductComplex","String"),
                new Tag(1598,"RelationshipRiskSecurityGroup","String"),
                new Tag(1599,"RelationshipRiskCFICode","String"),
                new Tag(1600,"RelationshipRiskSecurityType","String"),
                new Tag(1601,"RelationshipRiskSecuritySubType","String"),
                new Tag(1602,"RelationshipRiskMaturityMonthYear","MonthYear"),
                new Tag(1603,"RelationshipRiskMaturityTime","TZTimeOnly"),
                new Tag(1604,"RelationshipRiskRestructuringType","String"),
                new Tag(1605,"RelationshipRiskSeniority","String"),
                new Tag(1606,"RelationshipRiskPutOrCall","int"),
                new Tag(1607,"RelationshipRiskFlexibleIndicator","Boolean"),
                new Tag(1608,"RelationshipRiskCouponRate","Percentage"),
                new Tag(1609,"RelationshipRiskSecurityExchange","Exchange"),
                new Tag(1610,"RelationshipRiskSecurityDesc","String"),
                new Tag(1611,"RelationshipRiskInstrumentSettlType","String"),
                new Tag(1612,"RelationshipRiskInstrumentMultiplier","float"),
                new Tag(1613,"NoRelationshipRiskWarningLevels","NumInGroup"),
                new Tag(1614,"RelationshipRiskWarningLevelPercent","Percentage"),
                new Tag(1615,"RelationshipRiskWarningLevelName","String"),
                new Tag(1616,"RiskSecurityExchange","Exchange"),
                new Tag(1617,"StreamAsgnType","int"),
                new Tag(1618,"RelationshipRiskEncodedSecurityDescLen","Length"),
                new Tag(1619,"RelationshipRiskEncodedSecurityDesc","data"),
                new Tag(1620,"RiskEncodedSecurityDescLen","Length"),
                new Tag(1621,"RiskEncodedSecurityDesc","data"),
                new Tag(1273,"DerivativePositionLimit","int"),
                new Tag(1274,"DerivativeNTPositionLimit","int"),
                new Tag(1275,"DerivativeIssuer","String"),
                new Tag(1276,"DerivativeIssueDate","LocalMktDate"),
                new Tag(1277,"DerivativeEncodedIssuerLen","Length"),
                new Tag(1278,"DerivativeEncodedIssuer","data"),
                new Tag(1279,"DerivativeSecurityDesc","String"),
                new Tag(1280,"DerivativeEncodedSecurityDescLen","Length"),
                new Tag(1281,"DerivativeEncodedSecurityDesc","data"),
                new Tag(1282,"DerivativeSecurityXMLLen","Length"),
                new Tag(1283,"DerivativeSecurityXML","data"),
                new Tag(1284,"DerivativeSecurityXMLSchema","String"),
                new Tag(1285,"DerivativeContractSettlMonth","MonthYear"),
                new Tag(1286,"NoDerivativeEvents","NumInGroup"),
                new Tag(1287,"DerivativeEventType","int"),
                new Tag(1288,"DerivativeEventDate","LocalMktDate"),
                new Tag(1289,"DerivativeEventTime","UTCTimestamp"),
                new Tag(1290,"DerivativeEventPx","Price"),
                new Tag(1291,"DerivativeEventText","String"),
                new Tag(1292,"NoDerivativeInstrumentParties","NumInGroup"),
                new Tag(1293,"DerivativeInstrumentPartyID","String"),
                new Tag(1294,"DerivativeInstrumentPartyIDSource","String"),
                new Tag(1295,"DerivativeInstrumentPartyRole","int"),
                new Tag(1296,"NoDerivativeInstrumentPartySubIDs","NumInGroup"),
                new Tag(1297,"DerivativeInstrumentPartySubID","String"),
                new Tag(1298,"DerivativeInstrumentPartySubIDType","int"),
                new Tag(1299,"DerivativeExerciseStyle","char"),
                new Tag(1300,"MarketSegmentID","String"),
                new Tag(1301,"MarketID","Exchange"),
                new Tag(1302,"MaturityMonthYearIncrementUnits","int"),
                new Tag(1303,"MaturityMonthYearFormat","int"),
                new Tag(1304,"StrikeExerciseStyle","int"),
                new Tag(1305,"SecondaryPriceLimitType","int"),
                new Tag(1306,"PriceLimitType","int"),
                new Tag(1307,"DerivativeSecurityListRequestType","int"),
                new Tag(1308,"ExecInstValue","char"),
                new Tag(1309,"NoTradingSessionRules","NumInGroup"),
                new Tag(1310,"NoMarketSegments","NumInGroup"),
                new Tag(1311,"NoDerivativeInstrAttrib","NumInGroup"),
                new Tag(1312,"NoNestedInstrAttrib","NumInGroup"),
                new Tag(1313,"DerivativeInstrAttribType","int"),
                new Tag(1314,"DerivativeInstrAttribValue","String"),
                new Tag(1315,"DerivativePriceUnitOfMeasure","String"),
                new Tag(1316,"DerivativePriceUnitOfMeasureQty","Qty"),
                new Tag(1317,"DerivativeSettlMethod","char"),
                new Tag(1318,"DerivativePriceQuoteMethod","String"),
                new Tag(1319,"DerivativeValuationMethod","String"),
                new Tag(1320,"DerivativeListMethod","int"),
                new Tag(1321,"DerivativeCapPrice","Price"),
                new Tag(1322,"DerivativeFloorPrice","Price"),
                new Tag(1323,"DerivativePutOrCall","int"),
                new Tag(1324,"ListUpdateAction","char"),
                new Tag(1325,"ParentMktSegmID","String"),
                new Tag(1326,"TradingSessionDesc","String"),
                new Tag(1327,"TradSesUpdateAction","char"),
                new Tag(1328,"RejectText","String"),
                new Tag(1329,"FeeMultiplier","float"),
                new Tag(1330,"UnderlyingLegSymbol","String"),
                new Tag(1331,"UnderlyingLegSymbolSfx","String"),
                new Tag(1332,"UnderlyingLegSecurityID","String"),
                new Tag(1333,"UnderlyingLegSecurityIDSource","String"),
                new Tag(1334,"NoUnderlyingLegSecurityAltID","NumInGroup"),
                new Tag(1335,"UnderlyingLegSecurityAltID","String"),
                new Tag(1336,"UnderlyingLegSecurityAltIDSource","String"),
                new Tag(1337,"UnderlyingLegSecurityType","String"),
                new Tag(1338,"UnderlyingLegSecuritySubType","String"),
                new Tag(1339,"UnderlyingLegMaturityMonthYear","MonthYear"),
                new Tag(1340,"UnderlyingLegStrikePrice","Price"),
                new Tag(1341,"UnderlyingLegSecurityExchange","String"),
                new Tag(1342,"NoOfLegUnderlyings","NumInGroup"),
                new Tag(1343,"UnderlyingLegPutOrCall","int"),
                new Tag(1344,"UnderlyingLegCFICode","String"),
                new Tag(1345,"UnderlyingLegMaturityDate","LocalMktDate"),
                new Tag(1346,"ApplReqID","String"),
                new Tag(1347,"ApplReqType","int"),
                new Tag(1348,"ApplResponseType","int"),
                new Tag(1349,"ApplTotalMessageCount","int"),
                new Tag(1350,"ApplLastSeqNum","SeqNum"),
                new Tag(1351,"NoApplIDs","NumInGroup"),
                new Tag(1352,"ApplResendFlag","Boolean"),
                new Tag(1353,"ApplResponseID","String"),
                new Tag(1354,"ApplResponseError","int"),
                new Tag(1355,"RefApplID","String"),
                new Tag(1356,"ApplReportID","String"),
                new Tag(1357,"RefApplLastSeqNum","SeqNum"),
                new Tag(1358,"LegPutOrCall","int"),
                new Tag(1359,"EncodedSymbolLen","Length"),
                new Tag(1360,"EncodedSymbol","data"),
                new Tag(1361,"TotNoFills","int"),
                new Tag(1362,"NoFills","NumInGroup"),
                new Tag(1363,"FillExecID","String"),
                new Tag(1364,"FillPx","Price"),
                new Tag(1365,"FillQty","Qty"),
                new Tag(1366,"LegAllocID","String"),
                new Tag(1367,"LegAllocSettlCurrency","Currency"),
                new Tag(1368,"TradSesEvent","int"),
                new Tag(1369,"MassActionReportID","String"),
                new Tag(1370,"NoNotAffectedOrders","NumInGroup"),
                new Tag(1371,"NotAffectedOrderID","String"),
                new Tag(1372,"NotAffOrigClOrdID","String"),
                new Tag(1373,"MassActionType","int"),
                new Tag(1374,"MassActionScope","int"),
                new Tag(1375,"MassActionResponse","int"),
                new Tag(1376,"MassActionRejectReason","int"),
                new Tag(1377,"MultilegModel","int"),
                new Tag(1378,"MultilegPriceMethod","int"),
                new Tag(1379,"LegVolatility","float"),
                new Tag(1380,"DividendYield","Percentage"),
                new Tag(1381,"LegDividendYield","Percentage"),
                new Tag(1382,"CurrencyRatio","float"),
                new Tag(1383,"LegCurrencyRatio","float"),
                new Tag(1384,"LegExecInst","MultipleCharValue"),
                new Tag(1385,"ContingencyType","int"),
                new Tag(1386,"ListRejectReason","int"),
                new Tag(1387,"NoTrdRepIndicators","NumInGroup"),
                new Tag(1388,"TrdRepPartyRole","int"),
                new Tag(1389,"TrdRepIndicator","Boolean"),
                new Tag(1390,"TradePublishIndicator","int"),
                new Tag(1391,"UnderlyingLegOptAttribute","char"),
                new Tag(1392,"UnderlyingLegSecurityDesc","String"),
                new Tag(1393,"MarketReqID","String"),
                new Tag(1394,"MarketReportID","String"),
                new Tag(1395,"MarketUpdateAction","char"),
                new Tag(1396,"MarketSegmentDesc","String"),
                new Tag(1397,"EncodedMktSegmDescLen","Length"),
                new Tag(1398,"EncodedMktSegmDesc","data"),
                new Tag(1399,"ApplNewSeqNum","SeqNum"),
                new Tag(1400,"EncryptedPasswordMethod","int"),
                new Tag(1401,"EncryptedPasswordLen","Length"),
                new Tag(1402,"EncryptedPassword","data"),
                new Tag(1403,"EncryptedNewPasswordLen","Length"),
                new Tag(1404,"EncryptedNewPassword","data"),
                new Tag(1405,"UnderlyingLegMaturityTime","TZTimeOnly"),
                new Tag(1406,"RefApplExtID","int"),
                new Tag(1407,"DefaultApplExtID","int"),
                new Tag(1408,"DefaultCstmApplVerID","String"),
                new Tag(1409,"SessionStatus","int"),
                new Tag(1410,"DefaultVerIndicator","Boolean"),
                new Tag(1411,"Nested4PartySubIDType","int"),
                new Tag(1412,"Nested4PartySubID","String"),
                new Tag(1413,"NoNested4PartySubIDs","NumInGroup"),
                new Tag(1414,"NoNested4PartyIDs","NumInGroup"),
                new Tag(1415,"Nested4PartyID","String"),
                new Tag(1416,"Nested4PartyIDSource","char"),
                new Tag(1417,"Nested4PartyRole","int"),
                new Tag(1418,"LegLastQty","Qty"),
                new Tag(1419,"UnderlyingExerciseStyle","int"),
                new Tag(1420,"LegExerciseStyle","int"),
                new Tag(1421,"LegPriceUnitOfMeasure","String"),
                new Tag(1422,"LegPriceUnitOfMeasureQty","Qty"),
                new Tag(1423,"UnderlyingUnitOfMeasureQty","Qty"),
                new Tag(1424,"UnderlyingPriceUnitOfMeasure","String"),
                new Tag(1425,"UnderlyingPriceUnitOfMeasureQty","Qty"),
                new Tag(1426,"ApplReportType","int"),
                new Tag(1427,"SideExecID","String"),
                new Tag(1428,"OrderDelay","int"),
                new Tag(1429,"OrderDelayUnit","int"),
                new Tag(1430,"VenueType","char"),
                new Tag(1431,"RefOrdIDReason","int"),
                new Tag(1432,"OrigCustOrderCapacity","int"),
                new Tag(1433,"RefApplReqID","String"),
                new Tag(1434,"ModelType","int"),
                new Tag(1435,"ContractMultiplierUnit","int"),
                new Tag(1436,"LegContractMultiplierUnit","int"),
                new Tag(1437,"UnderlyingContractMultiplierUnit","int"),
                new Tag(1438,"DerivativeContractMultiplierUnit","int"),
                new Tag(1439,"FlowScheduleType","int"),
                new Tag(1440,"LegFlowScheduleType","int"),
                new Tag(1441,"UnderlyingFlowScheduleType","int"),
                new Tag(1442,"DerivativeFlowScheduleType","int"),
                new Tag(1443,"FillLiquidityInd","int"),
                new Tag(1444,"SideLiquidityInd","int"),
                new Tag(1445,"NoRateSources","NumInGroup"),
                new Tag(1446,"RateSource","int"),
                new Tag(1447,"RateSourceType","int"),
                new Tag(1448,"ReferencePage","String"),
                new Tag(1449,"RestructuringType","String"),
                new Tag(1450,"Seniority","String"),
                new Tag(1451,"NotionalPercentageOutstanding","Percentage"),
                new Tag(1452,"OriginalNotionalPercentageOutstanding","Percentage"),
                new Tag(1453,"UnderlyingRestructuringType","String"),
                new Tag(1454,"UnderlyingSeniority","String"),
                new Tag(1455,"UnderlyingNotionalPercentageOutstanding","Percentage"),


        };

            
            tagToObjectMap = new Dictionary<int, Tag>();
            
            foreach (Tag tag in tags)
            {
                allTagToObjectMap[tag.TagNum] = tag;
                tagToObjectMap[tag.TagNum] = tag;
            }
            headerTagToObjectMap = new Dictionary<int, Tag>();
            headerTagToObjectMap[35] = new Tag(35,"MsgType","String");
            headerTagToObjectMap[9] = new Tag(9,"BodyLength","Length");
            headerTagToObjectMap[34] = new Tag(34,"MsgSeqNum","SeqNum");
            headerTagToObjectMap[8] = new Tag(8,"BeginString","String");
            headerTagToObjectMap[49] = new Tag(49,"SenderCompID","String");
            headerTagToObjectMap[56] = new Tag(56,"TargetCompID","String");
            headerTagToObjectMap[115] = new Tag(115,"OnBehalfOfCompID","String");
            headerTagToObjectMap[128] = new Tag(128,"DeliverToCompID","String");
            headerTagToObjectMap[90] = new Tag(90,"SecureDataLen","Length");
            headerTagToObjectMap[91] = new Tag(91,"SecureData","data");
            headerTagToObjectMap[50] = new Tag(50,"SenderSubID","String");
            headerTagToObjectMap[142] = new Tag(142,"SenderLocationID","String");
            headerTagToObjectMap[57] = new Tag(57,"TargetSubID","String");
            headerTagToObjectMap[143] = new Tag(143,"TargetLocationID","String");
            headerTagToObjectMap[116] = new Tag(116,"OnBehalfOfSubID","String");
            headerTagToObjectMap[144] = new Tag(144,"OnBehalfOfLocationID","String");
            headerTagToObjectMap[129] = new Tag(129,"DeliverToSubID","String");
            headerTagToObjectMap[145] = new Tag(145,"DeliverToLocationID","String");
            headerTagToObjectMap[43] = new Tag(43,"PossDupFlag","Boolean");
            headerTagToObjectMap[97] = new Tag(97,"PossResend","Boolean");
            headerTagToObjectMap[52] = new Tag(52,"SendingTime","UTCTimestamp");
            headerTagToObjectMap[122] = new Tag(122,"OrigSendingTime","UTCTimestamp");
            headerTagToObjectMap[212] = new Tag(212,"XmlDataLen","Length");
            headerTagToObjectMap[213] = new Tag(213,"XmlData","data");
            headerTagToObjectMap[347] = new Tag(347,"MessageEncoding","String");
            headerTagToObjectMap[369] = new Tag(369,"LastMsgSeqNumProcessed","SeqNum");
            headerTagToObjectMap[1128] = new Tag(1128,"ApplVerID","String");
            headerTagToObjectMap[1129] = new Tag(1129,"CstmApplVerID","String");
            headerTagToObjectMap[1156] = new Tag(1156,"ApplExtID","int");
            foreach(KeyValuePair<int,Tag> pair in headerTagToObjectMap)
            {
                allTagToObjectMap[pair.Value.TagNum] = pair.Value;
            }
            msgValueMap = new Dictionary<int, Dictionary<string, string>>();
            msgValueMap[4] = new Dictionary<string, string>()
        {
            {"B","Buy"},{"S","Sell"},{"T","Trade"},{"X","Cross"},
    };
            msgValueMap[5] = new Dictionary<string, string>()
        {
            {"C","Cancel"},{"N","New"},{"R","Replace"},
    };
         
       
    msgValueMap[18] = new Dictionary<string, string>()
        {
            {"0","Stay on offer side"},{"1","Not held"},{"2","Work"},{"3","Go along"},{"4","Over the day"},{"5","Held"},{"6","Participate don't initiate"},{"7","Strict scale"},{"8","Try to scale"},{"9","Stay on bid side"},{"A","No cross"},{"B","OK to cross"},{"C","Call first"},{"D","Percent of volume"},{"E","Do not increase - DNI"},{"F","Do not reduce - DNR"},{"G","All or none - AON"},{"H","Reinstate on system failure"},{"I","Institutions only"},{"J","Reinstate on Trading Halt"},{"K","Cancel on Trading Halt"},{"L","Last peg (last sale)"},{"M","Mid-price peg (midprice of inside quote)"},{"N","Non-negotiable"},{"O","Opening peg"},{"P","Market peg"},{"Q","Cancel on system failure"},{"R","Primary peg (primary market - buy at bid/sell at offer)"},{"S","Suspend"},{"T","Fixed Peg to Local best bid or offer at time of order"},{"U","Customer Display Instruction (Rule 11Ac1-1/4)"},{"V","Netting (for Forex)"},{"W","Peg to VWAP"},{"X","Trade Along"},{"Y","Try To Stop"},{"Z","Cancel if not best"},{"a","Trailing Stop Peg"},{"b","Strict Limit"},{"c","Ignore Price Validity Checks"},{"d","Peg to Limit Price"},{"e","Work to Target Strategy"},{"f","Intermarket Sweep"},{"g","External Routing Allowed"},{"h","External Routing Not Allowed"},{"i","Imbalance Only"},{"j","Single execution requested for block trade"},{"k","Best Execution"},{"l","Suspend on system failure (mutually exclusive with H and Q)"},{"m","Suspend on Trading Halt (mutually exclusive with J and K)"},{"n","Reinstate on connection loss (mutually exclusive with o and p)"},{"o","Cancel on connection loss (mutually exclusive with n and p)"},{"p","Suspend on connection loss (mutually exclusive with n and o)"},{"q","Release from suspension (mutually exclusive with S)"},{"r","Execute as delta neutral using volatility provided"},{"s","Execute as duration neutral"},{"t","Execute as FX neutral"},
	};
msgValueMap[20] = new Dictionary<string, string>()
        {
            {"0","New"},{"1","Cancel"},{"2","Correct"},{"3","Status"},
	};
msgValueMap[21] = new Dictionary<string, string>()
        {
            {"1","Automated execution order, private, no Broker intervention"},{"2","Automated execution order, public, Broker intervention OK"},{"3","Manual order, best execution"},
	};
msgValueMap[22] = new Dictionary<string, string>()
        {
            {"1","CUSIP"},{"2","SEDOL"},{"3","QUIK"},{"4","ISIN number"},{"5","RIC code"},{"6","ISO Currency Code"},{"7","ISO Country Code"},{"8","Exchange Symbol"},{"9","Consolidated Tape Association (CTA) Symbol (SIAC CTS/CQS line format)"},{"A","Bloomberg Symbol"},{"B","Wertpapier"},{"C","Dutch"},{"D","Valoren"},{"E","Sicovam"},{"F","Belgian"},{"G","Common (Clearstream and Euroclear)"},{"H","Clearing House / Clearing Organization"},{"I","ISDA/FpML Product Specification (XML in EncodedSecurityDesc)"},{"J","Option Price Reporting Authority"},{"L","Letter of Credit"},{"K","ISDA/FpML Product URL (URL in SecurityID)"},{"M","Marketplace-assigned Identifier"},
	};
msgValueMap[25] = new Dictionary<string, string>()
        {
            {"H","High"},{"L","Low"},{"M","Medium"},
	};
msgValueMap[27] = new Dictionary<string, string>()
        {
            {"0","1000000000"},{"S","Small"},{"M","Medium"},{"L","Large"},{"U","Undisclosed Quantity"},
	};
msgValueMap[28] = new Dictionary<string, string>()
        {
            {"C","Cancel"},{"N","New"},{"R","Replace"},
	};
msgValueMap[29] = new Dictionary<string, string>()
        {
            {"1","Agent"},{"2","Cross as agent"},{"3","Cross as principal"},{"4","Principal"},
	};
msgValueMap[35] = new Dictionary<string, string>()
        {
            {"0","Heartbeat"},{"1","TestRequest"},{"2","ResendRequest"},{"3","Reject"},{"4","SequenceReset"},{"5","Logout"},{"6","IOI"},{"7","Advertisement"},{"8","ExecutionReport"},{"9","OrderCancelReject"},{"A","Logon"},{"AA","DerivativeSecurityList"},{"AB","NewOrderMultileg"},{"AC","MultilegOrderCancelReplace"},{"AD","TradeCaptureReportRequest"},{"AE","TradeCaptureReport"},{"AF","OrderMassStatusRequest"},{"AG","QuoteRequestReject"},{"AH","RFQRequest"},{"AI","QuoteStatusReport"},{"AJ","QuoteResponse"},{"AK","Confirmation"},{"AL","PositionMaintenanceRequest"},{"AM","PositionMaintenanceReport"},{"AN","RequestForPositions"},{"AO","RequestForPositionsAck"},{"AP","PositionReport"},{"AQ","TradeCaptureReportRequestAck"},{"AR","TradeCaptureReportAck"},{"AS","AllocationReport"},{"AT","AllocationReportAck"},{"AU","Confirmation_Ack"},{"AV","SettlementInstructionRequest"},{"AW","AssignmentReport"},{"AX","CollateralRequest"},{"AY","CollateralAssignment"},{"AZ","CollateralResponse"},{"B","News"},{"BA","CollateralReport"},{"BB","CollateralInquiry"},{"BC","NetworkCounterpartySystemStatusRequest"},{"BD","NetworkCounterpartySystemStatusResponse"},{"BE","UserRequest"},{"BF","UserResponse"},{"BG","CollateralInquiryAck"},{"BH","ConfirmationRequest"},{"BI","TradingSessionListRequest"},{"BJ","TradingSessionList"},{"BK","SecurityListUpdateReport"},{"BL","AdjustedPositionReport"},{"BM","AllocationInstructionAlert"},{"BN","ExecutionAcknowledgement"},{"BO","ContraryIntentionReport"},{"BP","SecurityDefinitionUpdateReport"},{"BQ","SettlementObligationReport"},{"BR","DerivativeSecurityListUpdateReport"},{"BS","TradingSessionListUpdateReport"},{"BT","MarketDefinitionRequest"},{"BU","MarketDefinition"},{"BV","MarketDefinitionUpdateReport"},{"BW","ApplicationMessageRequest"},{"BX","ApplicationMessageRequestAck"},{"BY","ApplicationMessageReport"},{"BZ","OrderMassActionReport"},{"C","Email"},{"CA","OrderMassActionRequest"},{"CB","UserNotification"},{"CC","StreamAssignmentRequest"},{"CD","StreamAssignmentReport"},{"CE","StreamAssignmentReportACK"},{"CF","PartyDetailsListRequest"},{"CG","PartyDetailsListReport"},{"D","NewOrderSingle"},{"E","NewOrderList"},{"F","OrderCancelRequest"},{"G","OrderCancelReplaceRequest"},{"H","OrderStatusRequest"},{"J","AllocationInstruction"},{"K","ListCancelRequest"},{"L","ListExecute"},{"M","ListStatusRequest"},{"N","ListStatus"},{"P","AllocationInstructionAck"},{"Q","DontKnowTradeDK"},{"R","QuoteRequest"},{"S","Quote"},{"T","SettlementInstructions"},{"V","MarketDataRequest"},{"W","MarketDataSnapshotFullRefresh"},{"X","MarketDataIncrementalRefresh"},{"Y","MarketDataRequestReject"},{"Z","QuoteCancel"},{"a","QuoteStatusRequest"},{"b","MassQuoteAcknowledgement"},{"c","SecurityDefinitionRequest"},{"d","SecurityDefinition"},{"e","SecurityStatusRequest"},{"f","SecurityStatus"},{"g","TradingSessionStatusRequest"},{"h","TradingSessionStatus"},{"i","MassQuote"},{"j","BusinessMessageReject"},{"k","BidRequest"},{"l","BidResponse"},{"m","ListStrikePrice"},{"n","XML_non_FIX"},{"o","RegistrationInstructions"},{"p","RegistrationInstructionsResponse"},{"q","OrderMassCancelRequest"},{"r","OrderMassCancelReport"},{"s","NewOrderCross"},{"t","CrossOrderCancelReplaceRequest"},{"u","CrossOrderCancelRequest"},{"v","SecurityTypeRequest"},{"w","SecurityTypes"},{"x","SecurityListRequest"},{"y","SecurityList"},{"z","DerivativeSecurityListRequest"},
	};
msgValueMap[39] = new Dictionary<string, string>()
        {
            {"0","New"},{"1","Partially filled"},{"2","Filled"},{"3","Done for day"},{"4","Canceled"},{"5","Replaced (No longer used)"},{"6","Pending Cancel (i.e. result of Order Cancel Request <F>"},{"7","Stopped"},{"8","Rejected"},{"9","Suspended"},{"A","Pending New"},{"B","Calculated"},{"C","Expired"},{"D","Accepted for Bidding"},{"E","Pending Replace (i.e. result of Order Cancel/Replace Request <G>"},
	};
msgValueMap[40] = new Dictionary<string, string>()
        {
            {"1","Market"},{"2","Limit"},{"3","Stop / Stop Loss"},{"4","Stop Limit"},{"5","Market On Close (No longer used)"},{"6","With Or Without"},{"7","Limit Or Better"},{"8","Limit With Or Without"},{"9","On Basis"},{"A","On Close (No longer used)"},{"B","Limit On Close (No longer used)"},{"C","Forex Market (No longer used)"},{"D","Previously Quoted"},{"E","Previously Indicated"},{"F","Forex Limit (No longer used)"},{"G","Forex Swap"},{"H","Forex Previously Quoted (No longer used)"},{"I","Funari"},{"J","Market If Touched (MIT)"},{"K","Market With Left Over as Limit"},{"L","Previous Fund Valuation Point"},{"M","Next Fund Valuation Point"},{"P","Pegged"},{"Q","Counter-Order Selection"},
	};
msgValueMap[43] = new Dictionary<string, string>()
        {
            {"N","Original transmission"},{"Y","Possible duplicate"},
	};
msgValueMap[47] = new Dictionary<string, string>()
        {
            {   "A" ,"Agency single order" },
    {"B", "Short exempt transaction (refer to A type)" },
    { "C", "Program Order, non-index arb, for Member firm/ org" },
    {"D", "Program Order, index arb, for Member firm/ org" },
    { "E", "Short Exempt Transaction for Principal(was incorrectly identified in the FIX spec as \"Registered Equity Market Maker trades\"" },
    { "F", "Short exempt transaction(refer to W type)" },
    { "H", "Short exempt transaction(refer to I type)" },
    { "I", "Individual Investor, single order" },
    { "J", "Program Order, index arb, for individual customer" },
    { "K", "Program Order, non - index arb, for individual customer" },
    { "L", "Short exempt transaction for member competing market - maker affiliated with the firm clearing the trade(refer to P and O types)" },
    { "M", "Program Order, index arb, for other member" },
    { "N", "Program Order, non - index arb, for other member" },
    { "O", "Proprietary transactions for competing market-maker that is affiliated with the clearing member(was incorrectly identified in the FIX spec as \"Competing dealer trades\")" },
    { "P", "Principal" },
    { "R", "Transactions for the account of a non - member competing market maker(was incorrectly identified in the FIX spec as \"Competing dealer trades\")" },
    { "S", "Specialist trades" },
    { "T", "Competing dealer trades" },
    { "U", "Program Order, index arb, for other agency" },
    { "W", "All other orders as agent for other member" },
    { "X", "Short exempt transaction for member competing market - maker not affiliated with the firm clearing the trade(refer to W and T types)" },
    { "Y", "Program Order, non - index arb, for other agency" },
    { "Z", "Short exempt transaction for non - member competing market - maker(refer to A and R types)" }
    };

            msgValueMap[54] = new Dictionary<string, string>()
        {
            {"1","Buy"},{"2","Sell"},{"3","Buy minus"},{"4","Sell plus"},{"5","Sell short"},{"6","Sell short exempt"},{"7","Undisclosed (valid for IOI and List Order messages only)"},{"8","Cross"},{"9","Cross short"},{"A","Cross short exempt"},{"B","As Defined"},{"C","Opposite"},{"D","Subscribe"},{"E","Redeem"},{"F","Lend (FINANCING - identifies direction of collateral)"},{"G","Borrow (FINANCING - identifies direction of collateral)"},
	};
msgValueMap[59] = new Dictionary<string, string>()
        {
            {"0","Day"},{"1","Good Till Cancel (GTC)"},{"2","At the Opening (OPG)"},{"3","Immediate Or Cancel (IOC)"},{"4","Fill Or Kill (FOK)"},{"5","Good Till Crossing (GTX)"},{"6","Good Till Date (GTD)"},{"7","At the Close"},{"8","Good Through Crossing"},{"9","At Crossing"},
	};
msgValueMap[61] = new Dictionary<string, string>()
        {
            {"0","Normal"},{"1","Flash"},{"2","Background"},
	};

msgValueMap[71] = new Dictionary<string, string>()
        {
            {"0","New"},{"1","Replace"},{"2","Cancel"},{"3","Preliminary (without MiscFees and NetMoney) (Removed/Replaced)"},{"4","Calculated (includes MiscFees and NetMoney) (Removed/Replaced)"},{"5","Calculated without Preliminary (sent unsolicited by broker, includes MiscFees and NetMoney) (Removed/Replaced)"},{"6","Reversal"},
	};
msgValueMap[72] = new Dictionary<string, string>()
        {
            {"      Reference identifier to be used with AllocTransType (71)","Replace or Cancel."},
	};
msgValueMap[77] = new Dictionary<string, string>()
        {
            {"C","Close"},{"D","Default"},{"F","FIFO"},{"N","Close but notify on open"},{"O","Open"},{"R","Rolled"},
	};
msgValueMap[81] = new Dictionary<string, string>()
        {
            {"0","Regular"},{"1","Soft Dollar"},{"2","Step-In"},{"3","Step-Out"},{"4","Soft-dollar Step-In"},{"5","Soft-dollar Step-Out"},{"6","Plan Sponsor"},
	};
msgValueMap[87] = new Dictionary<string, string>()
        {
            {"0","accepted (successfully processed)"},{"1","block level reject"},{"2","account level reject"},{"3","received (received, not yet processed)"},{"4","incomplete"},{"5","rejected by intermediary"},{"6","allocation pending"},{"7","reversed"},
	};
msgValueMap[88] = new Dictionary<string, string>()
        {
            {"0","Unknown account(s)"},{"1","Incorrect quantity"},{"10","Unknown or stale ExecID"},{"11","Mismatched data"},{"12","Unknown ClOrdID"},{"13","Warehouse request rejected"},{"2","Incorrect averageg price"},{"3","Unknown executing broker mnemonic"},{"4","Commission difference"},{"5","Unknown OrderID (37)"},{"6","Unknown ListID (66)"},{"7","Other (further in Text (58))"},{"8","Incorrect allocated quantity"},{"9","Calculation difference"},{"99","Other"},
	};
msgValueMap[94] = new Dictionary<string, string>()
        {
            {"0","New"},{"1","Reply"},{"2","Admin Reply"},
	};
msgValueMap[97] = new Dictionary<string, string>()
        {
            {"N","Original Transmission"},{"Y","Possible Resend"},
	};
msgValueMap[98] = new Dictionary<string, string>()
        {
            {"0","None / Other"},{"1","PKCS (Proprietary)"},{"2","DES (ECB Mode)"},{"3","PKCS / DES (Proprietary)"},{"4","PGP / DES (Defunct)"},{"5","PGP / DES-MD5 (See app note on FIX web site)"},{"6","PEM / DES-MD5 (see app note on FIX web site)"},
	};
msgValueMap[102] = new Dictionary<string, string>()
        {
            {"0","Too late to cancel"},{"1","Unknown order"},{"2","Broker / Exchange Option"},{"3","Order already in Pending Cancel or Pending Replace status"},{"4","Unable to process Order Mass Cancel Request"},{"5","OrigOrdModTime (586) did not match last TransactTime (60) of order"},{"6","Duplicate ClOrdID (11) received"},{"99","Other"},{"18","Invalid price increment"},{"7","Price exceeds current price"},{"8","Price exceeds current price band"},
	};
msgValueMap[103] = new Dictionary<string, string>()
        {
            {"0","Broker / Exchange option"},{"1","Unknown symbol"},{"10","Invalid Investor ID"},{"11","Unsupported order characteristic"},{"12","Surveillence Option"},{"13","Incorrect quantity"},{"14","Incorrect allocated quantity"},{"15","Unknown account(s)"},{"2","Exchange closed"},{"3","Order exceeds limit"},{"4","Too late to enter"},{"5","Unknown order"},{"6","Duplicate Order (e.g. dupe ClOrdID)"},{"7","Duplicate of a verbally communicated order"},{"8","Stale order"},{"9","Trade along required"},{"99","Other"},{"18","Invalid price increment"},{"16","Price exceeds current price band"},
	};
msgValueMap[104] = new Dictionary<string, string>()
        {
            {"A","All or None (AON)"},{"B","Market On Close (MOC)"},{"C","At the close"},{"D","VWAP (Volume Weighted Average Price)"},{"I","In touch with"},{"L","Limit"},{"M","More Behind"},{"O","At the Open"},{"P","Taking a Position"},{"Q","At the Market (previously called Current Quote)"},{"R","Ready to Trade"},{"S","Portfolio Shown"},{"T","Through the Day"},{"V","Versus"},{"W","Indication - Working Away"},{"X","Crossing Opportunity"},{"Y","At the Midpoint"},{"Z","Pre-open"},
	};
msgValueMap[113] = new Dictionary<string, string>()
        {
            {"N","Indicates the party sending message will report trade"},{"Y","Indicates the party receiving message must report trade"},
	};
msgValueMap[114] = new Dictionary<string, string>()
        {
            {"N","Indicates the broker is not required to locate"},{"Y","Indicates the broker is responsible for locating the stock"},
	};
msgValueMap[121] = new Dictionary<string, string>()
        {
            {"N","Do Not Execute Forex After Security Trade"},{"Y","Execute Forex After Security Trade"},
	};
msgValueMap[123] = new Dictionary<string, string>()
        {
            {"N","Sequence Reset, Ignore Msg Seq Num (N/A For FIXML - Not Used)"},{"Y","Gap Fill Message, Msg Seq Num Field Valid"},
	};
msgValueMap[127] = new Dictionary<string, string>()
        {
            {"A","Unknown Symbol"},{"B","Wrong Side"},{"C","Quantity Exceeds Order"},{"D","No Matching Order"},{"E","Price Exceeds Limit"},{"F","Calculation Difference"},{"Z","Other"},
	};
msgValueMap[130] = new Dictionary<string, string>()
        {
            {"N","Not Natural"},{"Y","Natural"},
	};
msgValueMap[139] = new Dictionary<string, string>()
        {
            {"1","Regulatory (e.g. SEC)"},{"10","Per transaction"},{"11","Conversion"},{"12","Agent"},{"2","Tax"},{"3","Local Commission"},{"4","Exchange Fees"},{"5","Stamp"},{"6","Levy"},{"7","Other"},{"8","Markup"},{"9","Consumption Tax"},{"13","Transfer Fee"},{"14","Security Lending"},
	};
msgValueMap[141] = new Dictionary<string, string>()
        {
            {"N","No"},{"Y","Yes, reset sequence numbers"},
	};
msgValueMap[150] = new Dictionary<string, string>()
        {
            {"0","New"},{"3","Done for day"},{"4","Canceled"},{"5","Replaced"},{"6","Pending Cancel (e.g. result of Order Cancel Request <F>"},{"7","Stopped"},{"8","Rejected"},{"9","Suspended"},{"A","Pending New"},{"B","Calculated"},{"C","Expired"},{"D","Restated ("},{"E","Pending Replace (e.g. result of Order Cancel/Replace Request <G>"},{"F","Trade (partial fill or fill)"},{"G","Trade Correct"},{"H","Trade Cancel"},{"I","Order Status"},{"J","Trade in a Clearing Hold"},{"K","Trade has been released to Clearing"},{"L","Triggered or Activated by System"},
	};

msgValueMap[156] = new Dictionary<string, string>()
        {
            {"M","Multiply"},{"D","Divide"},
	};
msgValueMap[160] = new Dictionary<string, string>()
        {
            {"0","Default (Replaced)"},{"1","Standing Instructions Provided"},{"2","Specific Allocation Account Overriding (Replaced)"},{"3","Specific Allocation Account Standing (Replaced)"},{"4","Specific Order for a single account (for CIV)"},{"5","Request reject"},
	};
msgValueMap[163] = new Dictionary<string, string>()
        {
            {"C","Cancel"},{"N","New"},{"R","Replace"},{"T","Restate"},
	};
msgValueMap[165] = new Dictionary<string, string>()
        {
            {"1","Broker's Instructions"},{"2","Institution's Instructions"},{"3","Investor (e.g. CIV use)"},
	};
msgValueMap[166] = new Dictionary<string, string>()
        {
            {"CED","CEDEL"},{"DTC","Depository Trust Company"},{"EUR","Euro clear"},{"FED","Federal Book Entry"},{"ISO_Country_Code","Local Market Settle Location"},{"PNY","Physical"},{"PTC","Participant Trust Company"},
	};
msgValueMap[167] = new Dictionary<string, string>()
        {
            {"ABS","Asset-backed Securities"},{"AMENDED","Amended &amp; Restated"},{"AN","Other Anticipation Notes (BAN, GAN, etc.)"},{"BA","Bankers Acceptance"},{"BN","Bank Notes"},{"BOX","Bill Of Exchanges"},{"BRADY","Brady Bond"},{"BRIDGE","Bridge Loan"},{"BUYSELL","Buy Sellback"},{"CB","Convertible Bond"},{"CD","Certificate Of Deposit"},{"CL","Call Loans"},{"CMBS","Corp. Mortgage-backed Securities"},{"CMO","Collateralized Mortgage Obligation"},{"COFO","Certificate Of Obligation"},{"COFP","Certificate Of Participation"},{"CORP","Corporate Bond"},{"CP","Commercial Paper"},{"CPP","Corporate Private Placement"},{"CS","Common Stock"},{"DEFLTED","Defaulted"},{"DINP","Debtor In Possession"},{"DN","Deposit Notes"},{"DUAL","Dual Currency"},{"EUCD","Euro Certificate Of Deposit"},{"EUCORP","Euro Corporate Bond"},{"EUCP","Euro Commercial Paper"},{"EUSOV","Euro Sovereigns *"},{"EUSUPRA","Euro Supranational Coupons *"},{"FAC","Federal Agency Coupon"},{"FADN","Federal Agency Discount Note"},{"FOR","Foreign Exchange Contract"},{"FORWARD","Forward"},{"FUT","Future"},{"GO","General Obligation Bonds"},{"IET","IOETTE Mortgage"},{"LOFC","Letter Of Credit"},{"LQN","Liquidity Note"},{"MATURED","Matured"},{"MBS","Mortgage-backed Securities"},{"MF","Mutual Fund"},{"MIO","Mortgage Interest Only"},{"MLEG","Multileg Instrument"},{"MPO","Mortgage Principal Only"},{"MPP","Mortgage Private Placement"},{"MPT","Miscellaneous Pass-through"},{"MT","Mandatory Tender"},{"MTN","Medium Term Notes"},{"NONE","No Security Type"},{"ONITE","Overnight"},{"OPT","Option"},{"PEF","Private Export Funding *"},{"PFAND","Pfandbriefe *"},{"PN","Promissory Note"},{"PS","Preferred Stock"},{"PZFJ","Plazos Fijos"},{"RAN","Revenue Anticipation Note"},{"REPLACD","Replaced"},{"REPO","Repurchase"},{"RETIRED","Retired"},{"REV","Revenue Bonds"},{"RVLV","Revolver Loan"},{"RVLVTRM","Revolver/Term Loan"},{"SECLOAN","Securities Loan"},{"SECPLEDGE","Securities Pledge"},{"SPCLA","Special Assessment"},{"SPCLO","Special Obligation"},{"SPCLT","Special Tax"},{"STN","Short Term Loan Note"},{"STRUCT","Structured Notes"},{"SUPRA","USD Supranational Coupons *"},{"SWING","Swing Line Facility"},{"TAN","Tax Anticipation Note"},{"TAXA","Tax Allocation"},{"TBA","To Be Announced"},{"TBILL","US Treasury Bill"},{"TBOND","US Treasury Bond"},{"TCAL","Principal Strip Of A Callable Bond Or Note"},{"TD","Time Deposit"},{"TECP","Tax Exempt Commercial Paper"},{"TERM","Term Loan"},{"TINT","Interest Strip From Any Bond Or Note"},{"TIPS","Treasury Inflation Protected Securities"},{"TNOTE","US Treasury Note"},{"TPRN","Principal Strip From A Non-Callable Bond Or Note"},{"TRAN","Tax Revenue Anticipation Note"},{"UST","US Treasury Note (Deprecated Value Use TNOTE)"},{"USTB","US Treasury Bill (Deprecated Value Use TBILL)"},{"VRDN","Variable Rate Demand Note"},{"WAR","Warrant"},{"WITHDRN","Withdrawn"},{"?","Wildcard entry for use on Security Definition Request"},{"XCN","Extended Comm Note"},{"XLINKD","Indexed Linked"},{"YANK","Yankee Corporate Bond"},{"YCD","Yankee Certificate Of Deposit"},{"OOP","Options on Physical - use not recommended"},{"OOF","Options on Futures"},{"CASH","Cash"},{"OOC","Options on Combo"},{"IRS","Interest Rate Swap"},{"BDN","Bank Depository Note"},{"CAMM","Canadian Money Markets"},{"CAN","Canadian Treasury Notes"},{"CTB","Canadian Treasury Bills"},{"CDS","Credit Default Swap"},{"CMB","Canadian Mortgage Bonds"},{"EUFRN","Euro Corporate Floating Rate Notes"},{"FRN","US Corporate Floating Rate Notes"},{"PROV","Canadian Provincial Bonds"},{"SLQN","Secured Liquidity Note"},{"TB","Treasury Bill - non US"},{"TLQN","Term Liquidity Note"},{"TMCP","Taxable Municipal CP"},{"FXNDF","Non-deliverable forward"},{"FXSPOT","FX Spot"},{"FXFWD","FX Forward"},{"FXSWAP","FX Swap"},
	};
msgValueMap[169] = new Dictionary<string, string>()
        {
            {"0","Other"},{"1","DTC SID"},{"2","Thomson ALERT"},{"3","A Global Custodian (StandInstDBName (70) must be provided)"},{"4","AccountNet"},
	};
msgValueMap[172] = new Dictionary<string, string>()
        {
            {"0","Versus.Payment: Deliver (if Sell) or Receive (if Buy) vs. (Against) Payment"},{"1","Free: Deliver (if Sell) or Receive (if Buy) Free"},{"2","Tri-Party"},{"3","Hold In Custody"},
	};
msgValueMap[197] = new Dictionary<string, string>()
        {
            {"0","FX Netting"},{"1","FX Swap"},
	};
msgValueMap[201] = new Dictionary<string, string>()
        {
            {"0","Put"},{"1","Call"},
	};
msgValueMap[203] = new Dictionary<string, string>()
        {
            {"0","Covered"},{"1","Uncovered"},
	};
msgValueMap[204] = new Dictionary<string, string>()
        {
            {"0","Customer"},{"1","Firm"},
	};
msgValueMap[208] = new Dictionary<string, string>()
        {
            {"N","Details shoult not be communicated"},{"Y","Details should be communicated"},
	};
msgValueMap[209] = new Dictionary<string, string>()
        {
            {"1","Match"},{"2","Forward"},{"3","Forward and Match"},
	};
msgValueMap[216] = new Dictionary<string, string>()
        {
            {"1","Target Firm"},{"2","Target List"},{"3","Block Firm"},{"4","Block List"},
	};
msgValueMap[219] = new Dictionary<string, string>()
        {
            {"1","CURVE"},{"2","5YR"},{"3","OLD5"},{"4","10YR"},{"5","OLD10"},{"6","30YR"},{"7","OLD30"},{"8","3MOLIBOR"},{"9","6MOLIBOR"},
	};
msgValueMap[221] = new Dictionary<string, string>()
        {
            {"EONIA","EONIA"},{"EUREPO","EUREPO"},{"Euribor","Euribor"},{"FutureSWAP","FutureSWAP"},{"LIBID","LIBID"},{"LIBOR","LIBOR (London Inter-Bank Offer)"},{"MuniAAA","MuniAAA"},{"OTHER","OTHER"},{"Pfandbriefe","Pfandbriefe"},{"SONIA","SONIA"},{"SWAP","SWAP"},{"Treasury","Treasury"},
	};

msgValueMap[228] = new Dictionary<string, string>()
        {
            {"      Qty * Factor * Price","Gross Trade Amount"},{"      (Qty * Price) * Factor","Nominal Value"},
	};


msgValueMap[235] = new Dictionary<string, string>()
        {
            {"AFTERTAX","After Tax Yield (Municipals)"},{"ANNUAL","Annual Yield"},{"ATISSUE","Yield At Issue"},{"AVGMATURITY","Yield To Avg Maturity"},{"BOOK","Book Yield"},{"CALL","Yield to Next Call"},{"CHANGE","Yield Change Since Close"},{"CLOSE","Closing Yield"},{"COMPOUND","Compound Yield"},{"CURRENT","Current Yield"},{"GOVTEQUIV","Gvnt Equivalent Yield"},{"GROSS","True Gross Yield"},{"INFLATION","Yield with Inflation Assumption"},{"INVERSEFLOATER","Inverse Floater Bond Yield"},{"LASTCLOSE","Most Recent Closing Yield"},{"LASTMONTH","Closing Yield Most Recent Month"},{"LASTQUARTER","Closing Yield Most Recent Quarter"},{"LASTYEAR","Closing Yield Most Recent Year"},{"LONGAVGLIFE","Yield to Longest Average Life"},{"MARK","Mark to Market Yield"},{"MATURITY","Yield to Maturity"},{"NEXTREFUND","Yield to Next Refund"},{"OPENAVG","Open Average Yield"},{"PREVCLOSE","Previous Close Yield"},{"PROCEEDS","Proceeds Yield"},{"PUT","Yield to Next Put"},{"SEMIANNUAL","Semi-annual Yield"},{"SHORTAVGLIFE","Yield to Shortest Average Life"},{"SIMPLE","Simple Yield"},{"TAXEQUIV","Tax Equivalent Yield"},{"TENDER","Yield to Tender Date"},{"TRUE","True Yield"},{"VALUE1_32","Yield Value Of 1/32"},{"WORST","Yield To Worst"},
	};
msgValueMap[258] = new Dictionary<string, string>()
        {
            {"N","Not Traded Flat"},{"Y","Traded Flat"},
	};
msgValueMap[263] = new Dictionary<string, string>()
        {
            {"0","Snapshot"},{"1","Snapshot + Updates (Subscribe)"},{"2","Disable previous Snapshot + Update Request (Unsubscribe)"},
	};
msgValueMap[265] = new Dictionary<string, string>()
        {
            {"0","Full refresh"},{"1","Incremental refresh"},
	};
msgValueMap[266] = new Dictionary<string, string>()
        {
            {"Specifies whether or not book entries should be aggregated. (Not specified)","broker option"},{"Y","book entries to be aggregated"},{"N","book entries should not be aggregated"},
	};
msgValueMap[269] = new Dictionary<string, string>()
        {
            {"0","Bid"},{"1","Offer"},{"2","Trade"},{"3","Index Value"},{"4","Opening Price"},{"5","Closing Price"},{"6","Settlement Price"},{"7","Trading Session High Price"},{"8","Trading Session Low Price"},{"9","Trading Session VWAP Price"},{"A","Imbalance"},{"B","Trade Volume"},{"C","Open Interest"},{"D","Composite Underlying Price"},{"E","Simulated Sell Price"},{"F","Simulated Buy Price"},{"G","Margin Rate"},{"H","Mid Price"},{"J","Empty Book"},{"K","Settle High Price"},{"L","Settle Low Price"},{"M","Prior Settle Price"},{"N","Session High Bid"},{"O","Session Low Offer"},{"P","Early Prices"},{"Q","Auction Clearing Price"},{"R","Daily value adjustment for long positions"},{"S","Swap Value Factor"},{"T","Cumulative Value Adjustment for long positions"},{"U","Daily Value Adjustment for Short Positions"},{"V","Cumulative Value Adjustment for Short Positions"},{"W","Fixing Price"},{"X","Cash Rate"},{"Y","Recovery Rate"},{"Z","Recovery Rate for Long"},{"a","Recovery Rate for Short"},
	};
msgValueMap[274] = new Dictionary<string, string>()
        {
            {"0","Plus Tick"},{"1","Zero-Plus Tick"},{"2","Minus Tick"},{"3","Zero-Minus Tick"},
	};
msgValueMap[276] = new Dictionary<string, string>()
        {
            {"0","Reserved SAM"},{"1","No Active SAM"},{"2","Restricted"},{"3","Rest of Book VWAP"},{"4","Better Prices in Conditional Orders"},{"5","Median Price"},{"6","Full Curve"},{"7","Flat Curve"},{"A","Open/Active"},{"B","Closed/Inactive"},{"C","Exchange Best"},{"D","Consolidated Best"},{"E","Locked"},{"F","Crossed"},{"G","Depth"},{"H","Fast Trading"},{"I","Non-Firm"},{"L","Manual/Slow Quote"},{"J","Outright Price"},{"K","Implied Price"},{"M","Depth on Offer"},{"N","Depth on Bid"},{"O","Closing"},{"P","News Dissemination"},{"Q","Trading Range"},{"R","Order Influx"},{"S","Due to Related"},{"T","News Pending"},{"U","Additional Info"},{"V","Additional Info due to related"},{"W","Resume"},{"X","View of Common"},{"Y","Volume Alert"},{"Z","Order Imbalance"},{"a","Equipment Changeover"},{"b","No Open / No Resume"},{"c","Regular ETH"},{"d","Automatic Execution"},{"e","Automatic Execution ETH"},{"f ","Fast Market ETH"},{"g","Inactive ETH"},{"h","Rotation"},{"i","Rotation ETH"},{"j","Halt"},{"k","Halt ETH"},{"l","Due to News Dissemination"},{"m","Due to News Pending"},{"n","Trading Resume"},{"o","Out of Sequence"},{"p","Bid Specialist"},{"q","Offer Specialist"},{"r","Bid Offer Specialist"},{"s","End of Day SAM"},{"t","Forbidden SAM"},{"u","Frozen SAM"},{"v","PreOpening SAM"},{"w","Opening SAM"},{"x","Open SAM"},{"y","Surveillance SAM"},{"z","Suspended SAM"},
	};
msgValueMap[277] = new Dictionary<string, string>()
        {
            {"0","Cancel"},{"1","Implied Trade"},{"2","Marketplace entered trade"},{"3","Mult Asset Class Multileg Trade"},{"4","Multileg-to-Multileg Trade"},{"A","Cash (only) Market"},{"B","Average Price Trade"},{"C","Cash Trade (same day clearing)"},{"D","Next Day (only)Market"},{"E","Opening/Reopening Trade Detail"},{"F","Intraday Trade Detail"},{"G","Rule 127 Trade (NYSE)"},{"H","Rule 155 Trade (AMEX)"},{"I","Sold Last (late reporting)"},{"J","Next Day Trade (next day clearing)"},{"K","Opened (late report of opened trade)"},{"L","Seller"},{"M","Sold (out of sequence)"},{"N","Stopped Stock (guarantee of price but does not execute the order)"},{"P","Imbalance More Buyers (cannot be used in combination with Q)"},{"Q","Imbalance More Sellers (cannot be used in combination with P)"},{"R","Opening Price"},{"Y","Trades resulting from manual/slow quote"},{"Z","Trades resulting from intermarket sweep"},{"S","Bargain Condition (LSE)"},{"T","Converted Price Indicator"},{"U","Exchange Last"},{"V","Final Price of Session"},{"W","Ex-pit"},{"X","Crossed"},{"a","Volume Only"},{"b","Direct Plus"},{"c","Acquisition"},{"d","Bunched"},{"e","Distribution"},{"f","Bunched Sale"},{"g","Split Trade"},{"h","Cancel Stopped"},{"i","Cancel ETH"},{"j","Cancel Stopped ETH"},{"k","Out of Sequence ETH"},{"l","Cancel Last ETH"},{"m","Sold Last Sale ETH"},{"n","Cancel Last"},{"o","Sold Last Sale"},{"p","Cancel Open"},{"q","Cancel Open ETH"},{"r","Opened Sale ETH"},{"s","Cancel Only"},{"t","Cancel Only ETH"},{"u","Late Open ETH"},{"v","Auto Execution ETH"},{"w","Reopen"},{"x","Reopen ETH"},{"y","Adjusted"},{"z","Adjusted ETH"},{"AA","Spread"},{"AB","Spread ETH"},{"AC","Straddle"},{"AD","Straddle ETH"},{"AE","Stopped"},{"AF","Stopped ETH"},{"AG","Regular ETH"},{"AH","Combo"},{"AI","Combo ETH"},{"AJ","Official Closing Price"},{"AK","Prior Reference Price"},{"AL","Stopped Sold Last"},{"AM","Stopped Out of Sequence"},{"AN","Offical Closing Price (duplicate enumeration - use 'AJ' instead)"},{"AO","Crossed (duplicate enumeration - use 'X' instead)"},{"AP","Fast Market"},{"AQ","Automatic Execution"},{"AR","Form T"},{"AS","Basket Index"},{"AT","Burst Basket"},{"AV","Outside Spread"},
	};
msgValueMap[279] = new Dictionary<string, string>()
        {
            {"0","New"},{"1","Change"},{"2","Delete"},{"3","Delete Thru"},{"4","Delete From"},{"5","Overlay"},
	};
msgValueMap[281] = new Dictionary<string, string>()
        {
            {"0","Unknown symbol"},{"1","Duplicate MDReqID"},{"2","Insufficient Bandwidth"},{"3","Insufficient Permissions"},{"4","Unsupported SubscriptionRequestType"},{"5","Unsupported MarketDepth"},{"6","Unsupported MDUpdateType"},{"7","Unsupported AggregatedBook"},{"8","Unsupported MDEntryType"},{"9","Unsupported TradingSessionID"},{"A","Unsupported Scope"},{"B","Unsupported OpenCloseSettleFlag"},{"C","Unsupported MDImplicitDelete"},{"D","Insufficient credit"},
	};
msgValueMap[285] = new Dictionary<string, string>()
        {
            {"0","Cancellation / Trade Bust"},{"1","Error"},
	};
msgValueMap[286] = new Dictionary<string, string>()
        {
            {"0","Daily Open / Close / Settlement entry"},{"1","Session Open / Close / Settlement entry"},{"2","Delivery Settlement entry"},{"3","Expected entry"},{"4","Entry from previous business day"},{"5","Theoretical Price value"},
	};
msgValueMap[291] = new Dictionary<string, string>()
        {
            {"1","Bankrupt"},{"2","Pending delisting"},{"3","Restricted"},
	};
msgValueMap[292] = new Dictionary<string, string>()
        {
            {"A","Ex-Dividend"},{"B","Ex-Distribution"},{"C","Ex-Rights"},{"D","New"},{"E","Ex-Interest"},{"F","Cash Dividend"},{"G","Stock Dividend"},{"H","Non-Integer Stock Split"},{"I","Reverse Stock Split"},{"J","Standard-Integer Stock Split"},{"K","Position Consolidation"},{"L","Liquidation Reorganization"},{"M","Merger Reorganization"},{"N","Rights Offering"},{"O","Shareholder Meeting"},{"P","Spinoff"},{"Q","Tender Offer"},{"R","Warrant"},{"S","Special Action"},{"T","Symbol Conversion"},{"U","CUSIP / Name Change"},{"V","Leap Rollover"},{"W","Succession Event"},
	};
msgValueMap[297] = new Dictionary<string, string>()
        {
            {"0","Accepted"},{"1","Cancel for Symbol(s)"},{"10","Pending"},{"11","Pass"},{"12","Locked Market Warning"},{"13","Cross Market Warning"},{"14","Canceled Due To Lock Market"},{"15","Canceled Due To Cross Market"},{"2","Canceled for Security Type(s)"},{"3","Canceled for Underlying"},{"4","Canceled All"},{"5","Rejected"},{"6","Removed from Market"},{"7","Expired"},{"8","Query"},{"9","Quote Not Found"},{"16","Active"},{"17","Canceled"},{"18","Unsolicited Quote Replenishment"},{"19","Pending End Trade"},{"20","Too Late to End"},
	};
msgValueMap[298] = new Dictionary<string, string>()
        {
            {"1","Cancel for one or more securities"},{"2","Cancel for Security Type(s)"},{"3","Cancel for underlying security"},{"4","Cancel All Quotes"},{"5","Cancel quote specified in QuoteID"},{"6","Cancel by QuoteType(537)"},{"7","Cancel for Security Issuer"},{"8","Cancel for Issuer of Underlying Security"},
	};
msgValueMap[300] = new Dictionary<string, string>()
        {
            {"1","Unknown Symbol (security)"},{"2","Exchange (Security) closed"},{"3","Quote <S>"},{"4","Too late to enter"},{"5","Unknown Quote <S>"},{"6","Duplicate Quote <S>"},{"7","Invalid bid/ask spread"},{"8","Invalid price"},{"9","Not authorized to quote security"},{"10","Price exceeds current price band"},{"11","Quote Locked - Unable to Update/Cancel"},{"12","Invalid or unknown Security Issuer"},{"13","Invalid or unknown Issuer of Underlying Security"},{"99","Other"},
	};
msgValueMap[301] = new Dictionary<string, string>()
        {
            {"0","No Acknowledgement"},{"1","Acknowledge only negative or erroneous quotes"},{"2","Acknowledge each quote message"},{"3","Summary Acknowledgement"},
	};
msgValueMap[303] = new Dictionary<string, string>()
        {
            {"1","Manual"},{"2","Automatic"},
	};
msgValueMap[310] = new Dictionary<string, string>()
        {
            {"                (see below for details concerning this fields use in conjunction with SecurityTyp","EPO)"},{"                The following applies when used in conjunction with SecurityTyp","EPO"},{"                Valid values for SecurityTyp","EPO:"},
	};

msgValueMap[323] = new Dictionary<string, string>()
        {
            {"1","Accept security proposal as-is"},{"2","Accept security proposal with revisions as indicated in the message"},{"3","List of security types returned per request"},{"4","List of securities returned per request"},{"5","Reject security proposal"},{"6","Cannot match selection criteria"},
	};
msgValueMap[325] = new Dictionary<string, string>()
        {
            {"N","Message is being sent as a result of a prior request"},{"Y","Message is being sent unsolicited"},
	};
msgValueMap[326] = new Dictionary<string, string>()
        {
            {"1","Opening delay"},{"2","Trading halt"},{"3","Resume"},{"4","No Open / No Resume"},{"5","Price indication"},{"6","Trading Range Indication"},{"7","Market Imbalance Buy"},{"8","Market Imbalance Sell"},{"9","Market on Close Imbalance Buy"},{"10","Market on Close Imbalance Sell"},{"11","(not assigned)"},{"12","No Market Imbalance"},{"13","No Market on Close Imbalance"},{"14","ITS Pre-opening"},{"15","New Price Indication"},{"16","Trade Dissemination Time"},{"17","Ready to trade (start of session)"},{"18","Not available for trading (end of session)"},{"19","Not traded on this market"},{"20","Unknown or Invalid"},{"21","Pre-open"},{"22","Opening Rotation"},{"23","Fast Market"},{"24","Pre-Cross - system is in a pre-cross state allowing market to respond to either side of cross"},{"25","Cross - system has crossed a percentage of the orders and allows market to respond prior to crossing remaining portion"},{"26","Post-close"},
	};
msgValueMap[327] = new Dictionary<string, string>()
        {
            {"0","News Dissemination"},{"1","Order Influx"},{"2","Order Imbalance"},{"3","Additional Information"},{"4","News Pending"},{"5","Equipment Changeover"},
	};
msgValueMap[328] = new Dictionary<string, string>()
        {
            {"N","Halt was not related to a halt of the common stock"},{"Y","Halt was due to common stock being halted"},
	};
msgValueMap[329] = new Dictionary<string, string>()
        {
            {"N","Halt was not related to a halt of the related security"},{"Y","Halt was due to related security being halted"},
	};
msgValueMap[334] = new Dictionary<string, string>()
        {
            {"1","Cancel"},{"2","Error"},{"3","Correction"},
	};

msgValueMap[338] = new Dictionary<string, string>()
        {
            {"1","Electronic"},{"2","Open Outcry"},{"3","Two Party"},
	};
msgValueMap[339] = new Dictionary<string, string>()
        {
            {"1","Testing"},{"2","Simulated"},{"3","Production"},
	};
msgValueMap[340] = new Dictionary<string, string>()
        {
            {"0","Unknown"},{"1","Halted"},{"2","Open"},{"3","Closed"},{"4","Pre-Open"},{"5","Pre-Close"},{"6","Request Rejected"},
	};
msgValueMap[373] = new Dictionary<string, string>()
        {
            {"0","Invalid Tag Number"},{"1","Required Tag Missing"},{"2","Tag not defined for this message type"},{"3","Undefined tag"},{"4","Tag specified without a value"},{"5","Value is incorrect (out of range) for this tag"},{"6","Incorrect data format for value"},{"7","Decryption problem"},{"8","Signature problem"},{"9"," CompID problem"},{"10","SendingTime <52>"},{"11","Invalid MsgType <35>"},{"12","XML Validation Error"},{"13","Tag appears more than once"},{"14","Tag specified out of required order"},{"15","Repeating group fields out of order"},{"16","Incorrect NumInGroup count for repeating group"},{"17","Non Data value includes field delimiter (<SOH> character)"},{"18","Invalid/Unsupported Application Version"},{"99","Other"},
	};
msgValueMap[374] = new Dictionary<string, string>()
        {
            {"C","Cancel"},{"N","New"},
	};
msgValueMap[377] = new Dictionary<string, string>()
        {
            {"N","Was not solicited"},{"Y","Was solicited"},
	};

msgValueMap[380] = new Dictionary<string, string>()
        {
            {"0","Other"},{"1","Unknown ID"},{"2","Unknown Security"},{"3","Unsupported Message Type"},{"4","Application not available"},{"5","Conditionally required field missing"},{"6","Not Authorized"},{"7","DeliverTo firm not available at this time"},{"18","Invalid price increment"},
	};
msgValueMap[385] = new Dictionary<string, string>()
        {
            {"R","Receive"},{"S","Send"},
	};
msgValueMap[388] = new Dictionary<string, string>()
        {
            {"0","Related to displayed price"},{"1","Related to market price"},{"2","Related to primary price"},{"3","Related to local primary price"},{"4","Related to midpoint price"},{"5","Related to last trade price"},{"6","Related to VWAP"},{"7","Average Price Guarantee"},
	};
msgValueMap[394] = new Dictionary<string, string>()
        {
            {"1","Non Disclosed style (e.g. US/European)"},{"2","Disclosed sytle (e.g. Japanese)"},{"3","No bidding process"},
	};
msgValueMap[399] = new Dictionary<string, string>()
        {
            {"1","Sector"},{"2","Country"},{"3","Index"},
	};

msgValueMap[401] = new Dictionary<string, string>()
        {
            {"1","Side Value 1"},{"2","Side Value 2"},
	};
msgValueMap[409] = new Dictionary<string, string>()
        {
            {"1","5-day moving average"},{"2","20-day moving average"},{"3","Normal market size"},{"4","Other"},
	};
msgValueMap[411] = new Dictionary<string, string>()
        {
            {"N","False"},{"Y","True"},
	};

msgValueMap[416] = new Dictionary<string, string>()
        {
            {"1","Net"},{"2","Gross"},
	};
msgValueMap[418] = new Dictionary<string, string>()
        {
            {"A","Agency"},{"G","VWAP Guarantee"},{"J","Guaranteed Close"},{"R","Risk Trade"},
	};
msgValueMap[419] = new Dictionary<string, string>()
        {
            {"2","Closing price at morningn session"},{"3","Closing price"},{"4","Current price"},{"5","SQ"},{"6","VWAP through a day"},{"7","VWAP through a morning session"},{"8","VWAP through an afternoon session"},{"9","VWAP through a day except YORI (an opening auction)"},{"A","VWAP through a morning session except YORI (an opening auction)"},{"B","VWAP through an afternoon session except YORI (an opening auction)"},{"C","Strike"},{"D","Open"},{"Z","Others"},
	};
msgValueMap[423] = new Dictionary<string, string>()
        {
            {"1","Percentage (i.e. percent of par"},{"2","Per unit"},{"3","Fixed amount (absolute value)"},{"4","Discount"},{"5","Premium"},{"6","Spread"},{"7","TED Price"},{"8","TED Yield"},{"9","Yield"},{"10","Fixed cabinet trade price"},{"11","Variable cabinet trade price"},{"13","Product ticks in halfs"},{"14","Product ticks in fourths"},{"15","Product ticks in eights"},{"16","Product ticks in sixteenths"},{"17","Product ticks in thirty-seconds"},{"18","Product ticks in sixty-forths"},{"19","Product ticks in one-twenty-eights"},
	};

msgValueMap[427] = new Dictionary<string, string>()
        {
            {"0","Book out all trades on day of execution"},{"1","Accumulate exectuions until forder is filled or expires"},{"2","Accumulate until verballly notified otherwise"},
	};
msgValueMap[429] = new Dictionary<string, string>()
        {
            {"1","Ack"},{"2","Response"},{"3","Timed"},{"4","Exec Started"},{"5","All Done"},{"6","Alert"},
	};
msgValueMap[430] = new Dictionary<string, string>()
        {
            {"1","Net"},{"2","Gross"},
	};
msgValueMap[431] = new Dictionary<string, string>()
        {
            {"1","In bidding process"},{"2","Received for execution"},{"3","Executing"},{"4","Cancelling"},{"5","Alert"},{"6","All Done"},{"7","Reject"},
	};
msgValueMap[433] = new Dictionary<string, string>()
        {
            {"1","Immediate"},{"2","Wait for Execut Instruction (i.e. a List Execut message or phone call before proceeding with execution of the list)"},{"3","Exchange/switch CIV order - Sell driven"},{"4","Exchange/switch CIV order - Buy driven, cash top-up (i.e. additional cash will be provided to fulfill the order)"},{"5","Exchange/switch CIV order - Buy driven, cash withdraw (i.e. additional cash will not be provided to fulfill the order)"},
	};
msgValueMap[434] = new Dictionary<string, string>()
        {
            {"1","Order cancel request"},{"2","Order cancel/replace request"},
	};
msgValueMap[442] = new Dictionary<string, string>()
        {
            {"1","Single security (default if not specified)"},{"2","Individual leg of a multi-leg security"},{"3","Multi-leg security"},
	};
msgValueMap[447] = new Dictionary<string, string>()
        {
            {"1","Korean Investor ID"},{"2","Taiwanese Qualified Foreign Investor ID QFII/FID"},{"3","Taiwanese Trading Acct"},{"4","Malaysian Central Depository (MCD) number"},{"5","Chinese Investor ID"},{"6","UK National Insurance or Pension Number"},{"7","US Social Security Number"},{"8","US Employer or Tax ID Number"},{"9","Australian Business Number"},{"A","Australian Tax File Number"},{"B","BIC (Bank Identification Code - SWIFT managed) code (ISO9362 - See Appendix 6-B)"},{"C","Generally accepted market participant identifier (e.g. NASD mnemonic)"},{"D","Proprietary / Custom code"},{"E","ISO Country Code"},{"F","Settlement Entity Location (note if Local Market Settlement use E=ISO Country Code"},{"G","MIC (ISO 10383 - Market Identificer Code"},{"H","CSD participant/member code (e.g.. Euroclear, DTC, CREST or Kassenverein number)"},{"I","Directed broker three character acronym as defined in ISITC guidelines document"},
	};
msgValueMap[452] = new Dictionary<string, string>()
        {
            {"1","Executing Firm"},{"2","Broker of Credit"},{"3","Client ID"},{"4","Clearing Firm"},{"5","Investor ID"},{"6","Introducing Firm"},{"7","Entering Firm"},{"8","Locate / Lending Firm"},{"9","Fund Manager Client ID (for CIV)"},{"10","Settlement Location"},{"11","Order Origination Trader"},{"12","Executing Trader"},{"13","Order Origination Firm"},{"14","Giveup Clearing Firm"},{"15","Correspondent Clearing Firm"},{"16","Executing System"},{"17","Contra Firm"},{"18","Contra Clearing Firm"},{"19","Sponsoring Firm"},{"20","Underlying Contra Firm"},{"21","Clearing Organization"},{"22","Exchange"},{"24","Customer Account"},{"25","Correspondent Clearing Organization"},{"26","Correspondent Broker"},{"27","Buyer/Seller (Receiver/Deliverer)"},{"28","Custodian"},{"29","Intermediary"},{"30","Agent"},{"31","Sub-custodian"},{"32","Beneficiary"},{"33","Interested party"},{"34","Regulatory body"},{"35","Liquidity provider"},{"36","Entering Trader"},{"37","Contra Trader"},{"38","Position account"},{"39","Contra Investor ID"},{"40","Transfer to Firm"},{"41","Contra Position Account"},{"42","Contra Exchange"},{"43","Internal Carry Account"},{"44","Order Entry Operator ID"},{"45","Secondary Account Number"},{"46","Foreign Firm"},{"47","Third Party Allocation Firm"},{"48","Claiming Account"},{"49","Asset Manager"},{"50","Pledgor Account"},{"51","Pledgee Account"},{"52","Large Trader Reportable Account"},{"53","Trader mnemonic"},{"54","Sender Location"},{"55","Session ID"},{"56","Acceptable Counterparty"},{"57","Unacceptable Counterparty"},{"58","Entering Unit"},{"59","Executing Unit"},{"60","Introducing Broker"},{"61","Quote originator"},{"62","Report originator"},{"63","Systematic internaliser (SI)"},{"64","Multilateral Trading Facility (MTF)"},{"65","Regulated Market (RM)"},{"66","Market Maker"},{"67","Investment Firm"},{"68","Host Competent Authority (Host CA)"},{"69","Home Competent Authority (Home CA)"},{"70","Competent Authority of the most relevant market in terms of liquidity (CAL)"},{"71","Competent Authority of the Transaction (Execution) Venue (CATV) "},{"72","Reporting intermediary (medium/vendor via which report has been published)"},{"73","Execution Venue"},{"74","Market data entry originator"},{"75","Location ID"},{"76","Desk ID"},{"77","Market data market"},{"78","Allocation Entity"},{"79","Prime Broker providing General Trade Services"},{"80","Step-Out Firm (Prime Broker)"},{"81","BrokerClearingID"},{"82","Central Registration Depository (CRD)"},{"83","Clearing Account"},{"84","Acceptable Settling Counterparty"},{"85","Unacceptable Settling Counterparty"},
	};
msgValueMap[460] = new Dictionary<string, string>()
        {
            {"1","AGENCY"},{"10","MORTGAGE"},{"11","MUNICIPAL"},{"12","OTHER"},{"13","FINANCING"},{"2","COMMODITY"},{"3","CORPORATE"},{"4","CURRENCY"},{"5","EQUITY"},{"6","GOVERNMENT"},{"7","INDEX"},{"8","LOAN"},{"9","MONEYMARKET"},
	};
msgValueMap[464] = new Dictionary<string, string>()
        {
            {"N","Fales (Production)"},{"Y","True (Test)"},
	};
msgValueMap[465] = new Dictionary<string, string>()
        {
            {"1","SHARES"},{"2","BONDS"},{"3","CURRENTFACE"},{"4","ORIGINALFACE"},{"5","CURRENCY"},{"6","CONTRACTS"},{"7","OTHER"},{"8","PAR"},
	};
msgValueMap[468] = new Dictionary<string, string>()
        {
            {"0","Round to nearest"},{"1","Round down"},{"2","Round up"},
	};
msgValueMap[477] = new Dictionary<string, string>()
        {
            {"1","CREST"},{"10","BPAY"},{"11","High Value Clearing System HVACS"},{"12","Reinvest In Fund"},{"2","NSCC"},{"3","Euroclear"},{"4","Clearstream"},{"5","Cheque"},{"6","Telegraphic Transfer"},{"7","Fed Wire"},{"8","Direct Credit (BECS, BACS)"},{"9","ACH Credit"},
	};
msgValueMap[480] = new Dictionary<string, string>()
        {
            {"M","No - Waiver agreement"},{"N","No - Execution Only"},{"O","No - Institutional"},{"Y","Yes"},
	};
msgValueMap[481] = new Dictionary<string, string>()
        {
            {"1","Exempt - Below the Limit"},{"2","Exempt - Client Money Type exemption"},{"3","Exempt - Authorised Credit or financial institution"},{"N","Not Checked"},{"Y","Passed"},
	};
msgValueMap[484] = new Dictionary<string, string>()
        {
            {"B","Bid price"},{"C","Creation price"},{"D","Creation price plus adjustment percent"},{"E","Creation price plus adjustment amount"},{"O","Offer price"},{"P","Offer price minus adjustment percent"},{"Q","Offer price minus adjustment amount"},{"S","Single price"},
	};
msgValueMap[487] = new Dictionary<string, string>()
        {
            {"0","New"},{"1","Cancel"},{"2","Replace"},{"3","Release"},{"4","Reverse"},{"5","Cancel Due To Back Out of Trade"},
	};
msgValueMap[492] = new Dictionary<string, string>()
        {
            {"1","CREST"},{"10","Direct Credit (BECS)"},{"11","Credit Card"},{"12","ACH Debit"},{"13","ACH Credit"},{"14","BPAY"},{"15","High Value Clearing System (HVACS)"},{"2","NSCC"},{"3","Euroclear"},{"4","Clearstream"},{"5","Cheque"},{"6","Telegraphic Transfer"},{"7","Fed Wire"},{"8","Debit Card"},{"9","Direct Debit (BECS)"},
	};

msgValueMap[497] = new Dictionary<string, string>()
        {
            {"N","No"},{"Y","Yes"},
	};
msgValueMap[506] = new Dictionary<string, string>()
        {
            {"A","Accepted"},{"H","Held"},{"N","Reminder - i.e. Registration Instructions are still outstanding"},{"R","Rejected"},
	};
msgValueMap[507] = new Dictionary<string, string>()
        {
            {"1","Invalid/unacceptable Account Type"},{"10","Invalid/unaceeptable Investor ID Source"},{"11","Invalid/unacceptable Date Of Birth"},{"12","Invalid/unacceptable Investor Country Of Residence"},{"13","Invalid/unacceptable No Distrib Instns"},{"14","Invalid/unacceptable Distrib Percentage"},{"15","Invalid/unacceptable Distrib Payment Method"},{"16","Invalid/unacceptable Cash Distrib Agent Acct Name"},{"17","Invalid/unacceptable Cash Distrib Agent Code"},{"18","Invalid/unacceptable Cash Distrib Agent Acct Num"},{"2","Invalid/unacceptable Tax Exempt Type"},{"3","Invalid/unacceptable Ownership Type"},{"4","Invalid/unacceptable No Reg Details"},{"5","Invalid/unacceptable Reg Seq No"},{"6","Invalid/unacceptable Reg Details"},{"7","Invalid/unacceptable Mailing Details"},{"8","Invalid/unacceptable Mailing Instructions"},{"9","Invalid/unacceptable Investor ID"},{"99","Other"},
	};
msgValueMap[514] = new Dictionary<string, string>()
        {
            {"0","New"},{"1","Replace"},{"2","Cancel"},
	};
msgValueMap[517] = new Dictionary<string, string>()
        {
            {"2","Joint Trustees"},{"J","Joint Investors"},{"T","Tenants in Common"},
	};
msgValueMap[519] = new Dictionary<string, string>()
        {
            {"1","Commission amount (actual)"},{"10","Exit Charge Percent"},{"11","Fund-Based Renewal Commission Percent (a.k.a. Trail commission)"},{"12","Projected Fund Value (i.e. for investments intended to realise or exceed a specific future value)"},{"13","Fund-Based Renewal Commission Amount (based on Order value)"},{"14","Fund-Based Renewal Commission Amount (based on Projected Fund value)"},{"15","Net Settlement Amount"},{"2","Commission percent (actual)"},{"3","Initial Charge Amount"},{"4","Initial Charge Percent"},{"5","Discount Amount"},{"6","Discount Percent"},{"7","Dilution Levy Amount"},{"8","Dilution Levy Percent"},{"9","Exit Charge Amount"},
	};
msgValueMap[522] = new Dictionary<string, string>()
        {
            {"1","Individual Investor"},{"10","Networking Sub-account"},{"11","Non-profit organization"},{"12","Corporate Body"},{"13","Nominee"},{"2","Public Company"},{"3","Private Company"},{"4","Individual Trustee"},{"5","Company Trustee"},{"6","Pension Plan"},{"7","Custodian Under Gifts to Minors Act"},{"8","Trusts"},{"9","Fiduciaries"},
	};

msgValueMap[528] = new Dictionary<string, string>()
        {
            {"A","Agency"},{"G","Proprietary"},{"I","Individual"},{"P","Principal (Note for CMS purposes, Principal includes Proprietary)"},{"R","Riskless Principal"},{"W","Agent for Other Member"},
	};
msgValueMap[529] = new Dictionary<string, string>()
        {
            {"1","Program Trade"},{"2","Index Arbitrage"},{"3","Non-Index Arbitrage"},{"4","Competing Market Maker"},{"5","Acting as Market Maker or Specialist in the security"},{"6","Acting as Market Maker or Specialist in the underlying security of a derivative security"},{"7","Foreign Entity (of foreign government or regulatory jurisdiction)"},{"8","External Market Participant"},{"9","External Inter-connected Market Linkage"},{"A","Riskless Arbitrage"},{"B","Issuer Holding"},{"C","Issue Price Stabilization"},{"D","Non-algorithmic"},{"E","Algorithmic"},{"F","Cross"},
	};
msgValueMap[530] = new Dictionary<string, string>()
        {
            {"1","Cancel orders for a security"},{"2","Cancel orders for an underlying security"},{"3","Cancel orders for a Product"},{"4","Cancel orders for a CFICode"},{"5","Cancel orders for a SecurityType"},{"6","Cancel orders for a trading session"},{"7","Cancel all orders"},{"8","Cancel orders for a market"},{"9","Cancel orders for a market segment"},{"A","Cancel orders for a security group"},{"B","Cancel for Security Issuer"},{"C","Cancel for Issuer of Underlying Security"},
	};
msgValueMap[531] = new Dictionary<string, string>()
        {
            {"0","Cancel Request Rejected - See MassCancelRejectReason (532)"},{"1","Cancel orders for a security"},{"2","Cancel orders for an Underlying Security"},{"3","Cancel orders for a Product"},{"4","Cancel orders for a CFICode"},{"5","Cancel orders for a SecurityType"},{"6","Cancel orders for a trading session"},{"7","Cancel All Orders"},{"8","Cancel orders for a market"},{"9","Cancel orders for a market segment"},{"A","Cancel orders for a security group"},{"B","Cancel Orders for a Securities Issuer"},{"C","Cancel Orders for Issuer of Underlying Security"},
	};
msgValueMap[532] = new Dictionary<string, string>()
        {
            {"0","Mass Cancel Not Supported"},{"1","Invalid or Unknown Security"},{"2","Invalid or Unkown Underlying security"},{"3","Invalid or Unknown Product"},{"4","Invalid or Unknown CFICode"},{"5","Invalid or Unknown SecurityType"},{"6","Invalid or Unknown Trading Session"},{"99","Other"},{"7","Invalid or unknown Market"},{"8","Invalid or unkown Market Segment"},{"9","Invalid or unknown Security Group"},{"10","Invalid or unknown Security Issuer"},{"11","Invalid or unknown Issuer of Underlying Security"},
	};
msgValueMap[537] = new Dictionary<string, string>()
        {
            {"0","Indicative"},{"1","Tradeable"},{"2","Restricted Tradeable"},{"3","Counter (tradeable)"},
	};
msgValueMap[544] = new Dictionary<string, string>()
        {
            {"1","Cash"},{"2","Margin Open"},{"3","Margin Close"},
	};
msgValueMap[546] = new Dictionary<string, string>()
        {
            {"1","Local Market (Exchange, ECN, ATS)"},{"2","National"},{"3","Global"},
	};
msgValueMap[547] = new Dictionary<string, string>()
        {
            {"N","Server must send an explicit delete for bids or offers falling outside the requested MarketDepth of the request"},{"Y","Client has responsibility for implicitly deleting bids or offers falling outside the MarketDepth of the request"},
	};

msgValueMap[550] = new Dictionary<string, string>()
        {
            {"0","None"},{"1","Buy side is prioritized"},{"2","Sell side is prioritized"},
	};
msgValueMap[552] = new Dictionary<string, string>()
        {
            {"1","One Side"},{"2","Both Sides"},
	};
msgValueMap[559] = new Dictionary<string, string>()
        {
            {"0","Symbol"},{"1","SecurityType and/or CFICode"},{"2","Product"},{"3","TradingSessionID"},{"4","All Securities"},{"5","MarketID or MarketID + MarketSegmentID"},
	};
msgValueMap[560] = new Dictionary<string, string>()
        {
            {"0","Valid request"},{"1","Invalid or unsupported request"},{"2","No instruments found that match selection criteria"},{"3","Not authorized to retrieve instrument data"},{"4","Instrument data temporarily unavailable"},{"5","Request for instrument data not supported"},
	};
msgValueMap[563] = new Dictionary<string, string>()
        {
            {"0","Report by mulitleg security only (do not report legs)"},{"1","Report by multileg security and by instrument legs belonging to the multileg security"},{"2","Report by instrument legs belonging to the multileg security only (do not report status of multileg security)"},
	};
msgValueMap[567] = new Dictionary<string, string>()
        {
            {"1","Unknown or invalid TradingSessionID"},{"99","Other"},
	};
msgValueMap[569] = new Dictionary<string, string>()
        {
            {"0","All Trades"},{"1","Matched trades matching criteria provided on request (Parties, ExecID, TradeID, OrderID, Instrument, InputSource, etc.)"},{"2","Unmatched trades that match criteria"},{"3","Unreported trades that match criteria"},{"4","Advisories that match criteria"},
	};
msgValueMap[570] = new Dictionary<string, string>()
        {
            {"N","Not reported to counterparty"},{"Y","Perviously reported to counterparty"},
	};
msgValueMap[573] = new Dictionary<string, string>()
        {
            {"0","Compared, matched or affirmed"},{"1","Uncompared, unmatched, or unaffirmed"},{"2","Advisory or alert"},
	};

msgValueMap[575] = new Dictionary<string, string>()
        {
            {"N","Treat as round lot (default)"},{"Y","Treat as odd lot"},
	};
msgValueMap[577] = new Dictionary<string, string>()
        {
            {"0","Process normally"},{"1","Exclude from all netting"},{"10","Automatic give-up mode (trade give-up to the give-up destination number specified)"},{"11","Qualified Service Representative QSR"},{"12","Customer trade"},{"13","Self clearing"},{"2","Bilateral netting only"},{"3","Ex clearing"},{"4","Special trade"},{"5","Multilateral netting"},{"6","Clear against central counterparty"},{"7","Exclude from central counterparty"},{"8","Manual mode (pre-posting and/or pre-giveup)"},{"9","Automatic posting mode (trade posting to the position account number specified)"},
	};
msgValueMap[581] = new Dictionary<string, string>()
        {
            {"1","Account is carried on customer side of the books"},{"2","Account is carried on non-customer side of books"},{"3","House Trader"},{"4","Floor Trader"},{"6","Account is carried on non-customer side of books and is cross margined"},{"7","Account is house trader and is cross margined"},{"8","Joint back office account (JBO)"},
	};
msgValueMap[582] = new Dictionary<string, string>()
        {
            {"1","Member trading for their own account"},{"2","Clearing Firm trading for its proprietary account"},{"3","Member trading for another member"},{"4","All other"},
	};
msgValueMap[585] = new Dictionary<string, string>()
        {
            {"1","Status for orders for a Security"},{"2","Status for orders for an Underlying Security"},{"3","Status for orders for a Product"},{"4","Status for orders for a CFICode"},{"5","Status for orders for a SecurityType"},{"6","Status for orders for a trading session"},{"7","Status for all orders"},{"8","Status for orders for a PartyID"},{"9","Status for Security Issuer"},{"10","Status for Issuer of Underlying Security"},
	};
msgValueMap[589] = new Dictionary<string, string>()
        {
            {"0","Can trigger booking without reference to the order initiator (auto)"},{"1","Speak with order initiator before booking (speak first)"},{"2","Accumulate"},
	};
msgValueMap[590] = new Dictionary<string, string>()
        {
            {"0","Each partial execution is a bookable unit"},{"1","Aggregate partial executions on this order, and book one trade per order"},{"2","Aggregate executions for this symbol, side, and settlement date"},
	};
msgValueMap[591] = new Dictionary<string, string>()
        {
            {"0","Pro-rata"},{"1","Do not pro-rata - discuss first"},
	};
msgValueMap[625] = new Dictionary<string, string>()
        {
            {"1","Pre-Trading"},{"2","Opening or opening auction"},{"3","(Continuous) Trading"},{"4","Closing or closing auction"},{"5","Post-Trading"},{"6","Intraday Auction"},{"7","Quiescent"},
	};
msgValueMap[626] = new Dictionary<string, string>()
        {
            {"1","Calculated (includes MiscFees and NetMoney <118>"},{"2","Preliminary (without MiscFees and NetMoney <118>"},{"3","Sellside Calculated Using Preliminary (includes MiscFees and NetMoney <118>"},{"4","Sellside Calculated Without Preliminary (sent unsolicited by sellside, includes MiscFees and NetMoney <118>"},{"5","Ready-To-Book - Single Order"},{"6","Buyside Ready-To-Book - Combined Set of Orders (Replaced)"},{"7","Warehouse Instruction"},{"8","Request to Intermediary"},{"9","Accept"},{"10","Reject"},{"11","Accept Pending"},{"12","Incomplete Group"},{"13","Complete Group"},{"14","Reversal Pending"},
	};
msgValueMap[635] = new Dictionary<string, string>()
        {
            {"1","1st year delegate trading for own account"},{"2","2nd year delegate trading for own account"},{"3","3rd year delegate trading for own account"},{"4","4th year delegate trading for own account"},{"5","5th year delegate trading for own account"},{"9","6th year delegate trading for own account"},{"B","CBOE Member"},{"C","Non-member and Customer"},{"E","Equity Member and Clearing Member"},{"F","Full and Associate Member trading for own account and as floor brokers"},{"H","106.H and 106.J firms"},{"I","GIM, IDEM and COM Membership Interest Holders"},{"L","Lessee 106.F Employees"},{"M","All other ownership types"},
	};

msgValueMap[638] = new Dictionary<string, string>()
        {
            {"0","Priority unchanged"},{"1","Lost Priority as result of order change"},
	};
msgValueMap[650] = new Dictionary<string, string>()
        {
            {"N","Does not consitute a Legal Confirm"},{"Y","Legal Confirm"},
	};
msgValueMap[653] = new Dictionary<string, string>()
        {
            {"0","Pending Approval"},{"1","Approved (Accepted)"},{"2","Rejected"},{"3","Unauthorized Request"},{"4","Invalid Definition Request"},
	};
msgValueMap[658] = new Dictionary<string, string>()
        {
            {"1","Unknown Symbol (Security)"},{"2","Exchange (Security) Closed"},{"3","Quote Request <R>"},{"4","Too Late to enter"},{"5","Invalid Price"},{"6","Not Authorized To Request Quote"},{"7","No Match For Inquiry"},{"8","No Market For Instrument"},{"9","No Inventory"},{"10","Pass"},{"11","Insufficient credit"},{"99","Other"},
	};
msgValueMap[660] = new Dictionary<string, string>()
        {
            {"1","BIC"},{"2","SID Code"},{"3","TFM (GSPTA)"},{"4","OMGEO (Alert ID)"},{"5","DTCC Code"},{"99","Other (custom or proprietary)"},
	};
msgValueMap[665] = new Dictionary<string, string>()
        {
            {"1","Received"},{"2","Mismatched Account"},{"3","Missing Settlement Instructions"},{"4","Confirmed"},{"5","Request Rejected"},
	};
msgValueMap[666] = new Dictionary<string, string>()
        {
            {"0","New"},{"1","Replace"},{"2","Cancel"},
	};
msgValueMap[668] = new Dictionary<string, string>()
        {
            {"1","Book Entry (default)"},{"2","Bearer"},
	};
msgValueMap[690] = new Dictionary<string, string>()
        {
            {"1","Par For Par"},{"2","Modified Duration"},{"4","Risk"},{"5","Proceeds"},
	};
msgValueMap[692] = new Dictionary<string, string>()
        {
            {"1","Percent (percent of par)"},{"10","Yield"},{"2","Per Share (e.g. cents per share)"},{"3","Fixed Amount (absolute value)"},{"4","Discount - percentage points below par"},{"5","Premium - percentage points over par"},{"6","Spread - basis points relative to benchmark"},{"7","TED Price"},{"8","TED Yield"},{"9","Yield Spread (swaps)"},
	};
msgValueMap[694] = new Dictionary<string, string>()
        {
            {"1","Hit/Lift"},{"2","Counter"},{"3","Expired"},{"4","Cover"},{"5","Done Away"},{"6","Pass"},{"7","End Trade"},{"8","Timed Out"},
	};
msgValueMap[703] = new Dictionary<string, string>()
        {
            {"ALC","Allocation Trade Qty"},{"AS","Option Assignment"},{"ASF","As-of Trade Qty"},{"DLV","Delivery Qty"},{"ETR","Electronic Trade Qty"},{"EX","Option Exercise Qty"},{"FIN","End-of-Day Qty"},{"IAS","Intra-spread Qty"},{"IES","Inter-spread Qty"},{"PA","Adjustment Qty"},{"PIT","Pit Trade Qty"},{"SOD","Start-of-Day Qty"},{"SPL","Integral Split"},{"TA","Transaction from Assignment"},{"TOT","Total Transaction Qty"},{"TQ","Transaction Quantity"},{"TRF","Transfer Trade Qty"},{"TX","Transaction from Exercise"},{"XM","Cross Margin Qty"},{"RCV","Receive Quantity"},{"CAA","Corporate Action Adjustment"},{"DN","Delivery Notice Qty"},{"EP","Exchange for Physical Qty"},{"PNTN","Privately negotiated Trade Qty (Non-regulated)"},{"DLT","Net Delta Qty"},{"CEA","Credit Event Adjustment"},{"SEA","Succession Event Adjustment"},
	};
msgValueMap[706] = new Dictionary<string, string>()
        {
            {"0","Submitted"},{"1","Accepted"},{"2","Rejected"},
	};
msgValueMap[707] = new Dictionary<string, string>()
        {
            {"CASH","Cash Amount (Corporate Event)"},{"CRES","Cash Residual Amount"},{"FMTM","Final Mark-to-Market Amount"},{"IMTM","Incremental Mark-to-Market Amount"},{"PREM","Premium Amount"},{"SMTM","Start-of-Day Mark-to-Market Amount"},{"TVAR","Trade Variation Amount"},{"VADJ","Value Adjusted Amount"},{"SETL","Settlement Value"},{"ICPN","Initial Trade Coupon Amount"},{"ACPN","Accrued Coupon Amount"},{"CPN","Coupon Amount"},{"IACPN","Incremental Accrued Coupon"},{"CMTM","Collateralized Mark to Market"},{"ICMTM","Incremental Collateralized Mark to market"},{"DLV","Compensation Amount"},{"BANK","Total Banked Amount"},{"COLAT","Total Collateralized Amount"},
	};
msgValueMap[709] = new Dictionary<string, string>()
        {
            {"1","Exercise"},{"2","Do Not Exercise"},{"3","Position Adjustment"},{"4","Position Change Submission/Margin Disposition"},{"5","Pledge"},{"6","Large Trader Submission"},
	};
msgValueMap[712] = new Dictionary<string, string>()
        {
            {"1","New - used to increment the overall transaction quantity"},{"2","Replace - used to override the overall transaction quantity or specifi add messages based on the reference ID"},{"3","Cancel - used to remove the overall transaction or specific add messages based on reference ID"},{"4","Reverse - used to completelly back-out the transaction such that the transaction never existed"},
	};
msgValueMap[716] = new Dictionary<string, string>()
        {
            {"ITD","Intraday"},{"RTH","Regular Trading Hours"},{"ETH","Electronic Trading Hours"},{"EOD","End Of Day"},
	};
msgValueMap[718] = new Dictionary<string, string>()
        {
            {"0","Process Request As Margin Disposition"},{"1","Delta Plus"},{"2","Delta Minus"},{"3","Final"},
	};
msgValueMap[722] = new Dictionary<string, string>()
        {
            {"0","Accepted"},{"1","Accepted With Warnings"},{"2","Rejected"},{"3","Completed"},{"4","Completed With Warnings"},
	};
msgValueMap[723] = new Dictionary<string, string>()
        {
            {"0","Successful Completion - no warnings or errors"},{"1","Rejected"},{"99","Other"},
	};
msgValueMap[724] = new Dictionary<string, string>()
        {
            {"0","Positions"},{"1","Trades"},{"2","Exercises"},{"3","Assignments"},{"4","Settlement Activity"},{"5","Backout Message"},{"6","Delta Positions"},
	};

msgValueMap[728] = new Dictionary<string, string>()
        {
            {"0","Valid request"},{"1","Invalid or unsupported request"},{"2","No positions found that match criteria"},{"3","Not authorized to request positions"},{"4","Request for position not supported"},{"99","Other (use Text (58) in conjunction with this code for an explaination)"},
	};
msgValueMap[729] = new Dictionary<string, string>()
        {
            {"0","Completed"},{"1","Completed With Warnings"},{"2","Rejected"},
	};
msgValueMap[731] = new Dictionary<string, string>()
        {
            {"1","Final"},{"2","Theoretical"},
	};
msgValueMap[744] = new Dictionary<string, string>()
        {
            {"P","Pro-rata"},{"R","Random"},
	};
msgValueMap[747] = new Dictionary<string, string>()
        {
            {"A","Automatic"},{"M","Manual"},
	};
msgValueMap[749] = new Dictionary<string, string>()
        {
            {"0","Successful (default)"},{"1","Invalid or unknown instrument"},{"2","Invalid type of trade requested"},{"3","Invalid parties"},{"4","Invalid transport type requested"},{"5","Invalid destination requested"},{"8","TradeRequestType not supported"},{"9","Not authorized"},{"99","Other"},
	};
msgValueMap[750] = new Dictionary<string, string>()
        {
            {"0","Accepted"},{"1","Completed"},{"2","Rejected"},
	};
msgValueMap[751] = new Dictionary<string, string>()
        {
            {"0","Successful (default)"},{"1","Invalid party information"},{"2","Unknown instrument"},{"3","Unauthorized to report trades"},{"4","Invalid trade type"},{"99","Other"},
	};
msgValueMap[752] = new Dictionary<string, string>()
        {
            {"1","Single Security (default if not specified)"},{"2","Individual leg of a multileg security"},{"3","Multileg Security"},
	};

msgValueMap[770] = new Dictionary<string, string>()
        {
            {"1","Execution Time"},{"2","Time In"},{"3","Time Out"},{"4","Broker Receipt"},{"5","Broker Execution"},{"6","Desk Receipt"},{"7","Submission to Clearing"},
	};
msgValueMap[772] = new Dictionary<string, string>()
        {
            {"      Reference identifier to be used with ConfirmTransType (666)","Replace or Cancel"},
	};
msgValueMap[773] = new Dictionary<string, string>()
        {
            {"1","Status"},{"2","Confirmation"},{"3","Confirmation Request Rejected (reason can be stated in Text (58) field)"},
	};
msgValueMap[774] = new Dictionary<string, string>()
        {
            {"1","Mismatched account"},{"2","Missing settlement instructions"},{"99","Other"},
	};
msgValueMap[775] = new Dictionary<string, string>()
        {
            {"0","Regular booking"},{"1","CFD (Contract for difference)"},{"2","Total Return Swap"},
	};
msgValueMap[780] = new Dictionary<string, string>()
        {
            {"0","Use default instructions"},{"1","Derive from parameters provided"},{"2","Full details provided"},{"3","SSI DB IDs provided"},{"4","Phone for instructions"},
	};
msgValueMap[787] = new Dictionary<string, string>()
        {
            {"C","Cash"},{"S","Securities"},
	};
msgValueMap[788] = new Dictionary<string, string>()
        {
            {"1","Overnight"},{"2","Term"},{"3","Flexible"},{"4","Open"},
	};
msgValueMap[792] = new Dictionary<string, string>()
        {
            {"0","Unable to process request"},{"1","Unknown account"},{"2","No matching settlement instructions found"},{"99","Other"},
	};
msgValueMap[794] = new Dictionary<string, string>()
        {
            {"3","Sellside Calculated Using Preliminary (includes MiscFees and NetMoney)"},{"4","Sellside Calculated Without Preliminary (sent unsolicited by sellside, includes MiscFees and NetMoney)"},{"5","Warehouse Recap"},{"8","Request to Intermediary"},{"2","Preliminary Request to Intermediary"},{"9","Accept"},{"10","Reject"},{"11","Accept Pending"},{"12","Complete"},{"14","Reverse Pending"},
	};
msgValueMap[795] = new Dictionary<string, string>()
        {
            {"      Reference identifier to be used with AllocTransType (7)","Replace or Cancel"},
	};
msgValueMap[796] = new Dictionary<string, string>()
        {
            {"1","Original details incomplete/incorrect"},{"2","Change in underlying order details"},{"99","Other"},
	};
msgValueMap[798] = new Dictionary<string, string>()
        {
            {"1","Account is carried pn customer side of books"},{"2","Account is carried on non-customer side of books"},{"3","House trader"},{"4","Floor trader"},{"6","Account is carried on non-customer side of books and is cross margined"},{"7","Account is house trader and is cross margined"},{"8","Joint back office account (JBO)"},
	};


msgValueMap[814] = new Dictionary<string, string>()
        {
            {"0","No Action Taken"},{"1","Queue Flushed"},{"2","Overlay Last"},{"3","End Session"},
	};
msgValueMap[815] = new Dictionary<string, string>()
        {
            {"0","No Action Taken"},{"1","Queue Flushed"},{"2","Overlay Last"},{"3","End Session"},
	};
msgValueMap[819] = new Dictionary<string, string>()
        {
            {"0","No Average Pricing"},{"1","Trade is part of an average price group identified by the TradeLinkID (820)"},{"2","Last trade is the average price group identified by the TradeLinkID (820)"},
	};
msgValueMap[826] = new Dictionary<string, string>()
        {
            {"0","Allocation not required"},{"1","Allocation required (give-up trade) allocation information not provided (incomplete)"},{"2","Use allocation provided with the trade"},{"3","Allocation give-up executor"},{"4","Allocation from executor"},{"5","Allocation to claim account"},
	};

msgValueMap[828] = new Dictionary<string, string>()
        {
            {"0","Regular Trade"},{"1","Block Trade"},{"2","EFP (Exchange for physical)"},{"3","Transfer"},{"4","Late Trade"},{"5","T Trade"},{"6","Weighted Average Price Trade"},{"7","Bunched Trade"},{"8","Late Bunched Trade"},{"9","Prior Reference Price Trade"},{"10","After Hours Trade"},{"11","Exchange for Risk (EFR)"},{"12","Exchange for Swap (EFS )"},{"13","Exchange of Futures for (in Market) Futures (EFM ) (e,g, full sized for mini)"},{"14","Exchange of Options for Options (EOO)"},{"15","Trading at Settlement"},{"16","All or None"},{"17","Futures Large Order Execution"},{"18","Exchange of Futures for Futures (external market) (EFF)"},{"19","Option Interim Trade"},{"20","Option Cabinet Trade"},{"22","Privately Negotiated Trades"},{"23","Substitution of Futures for Forwards"},{"24","Error trade"},{"25","Special cum dividend (CD)"},{"26","Special ex dividend (XD)"},{"27","Special cum coupon (CC)"},{"28","Special ex coupon (XC)"},{"29","Cash settlement (CS)"},{"30","Special price (usually net- or all-in price) (SP)"},{"31","Guaranteed delivery (GD)"},{"32","Special cum rights (CR)"},{"33","Special ex rights (XR)"},{"34","Special cum capital repayments (CP)"},{"35","Special ex capital repayments (XP)"},{"36","Special cum bonus (CB)"},{"37","Special ex bonus (XB)"},{"38","Block trade (same as large trade)"},{"39","Worked principal trade (UK-specific)"},{"40","Block Trades - after market"},{"41","Name change"},{"42","Portfolio transfer"},{"43","Prorogation buy - Euronext Paris only."}
};
msgValueMap[829] = new Dictionary<string, string>()
        {
            {"0","CMTA"},{"1","Internal transfer or adjustment"},{"2","External transfer or transfer of account"},{"3","Reject for submitting side"},{"4","Advisory for contra side"},{"5","Offset due to an allocation"},{"6","Onset due to an allocation"},{"7","Differential spread"},{"8","Implied spread leg executed against an outright"},{"9","Transaction from exercise"},{"10","Transaction from assignment"},{"11","ACATS"},{"14","AI (Automated input facility disabled in response to an exchange request.)"},{"15","B "},{"28","RT(Risk transaction in a SEATS security, (excluding AIM security) reported by a market maker registered in that security)"},{"29","SW(Transactions resulting from stock swap or a stock switch (one report is required for each line of stock))"},{"30","T(If reporting a single protected transaction)"},{"31","WN(Worked principal notification for a single order book security)"},{"32","WT(Worked principal transaction (other than a portfolio transaction))"},{"33","Off Hours Trade"},{"34","On Hours Trade"},{"35","OTC Quote"},{"36","Converted SWAP"},{"37","Crossed Trade(X)"},{"38","Interim Protected Trade(I)"},{"39","Large in Scale(L)"},
	};
msgValueMap[835] = new Dictionary<string, string>()
        {
            {"0","Floating (default)"},{"1","Fixed"},
	};
msgValueMap[836] = new Dictionary<string, string>()
        {
            {"0","Price (default)"},{"1","Basis Points"},{"2","Ticks"},{"3","Price Tier / Level"},
	};
msgValueMap[837] = new Dictionary<string, string>()
        {
            {"0","Or better (default) - price improvement allowed"},{"1","Strict - limit is a strict limit"},{"2","Or worse - for a buy the peg limit is a minimum and for a sell the peg limit is a maximum (for use for orders which have a price range)"},
	};
msgValueMap[838] = new Dictionary<string, string>()
        {
            {"1","More aggressive - on a buy order round the price up to the nearest tick; on a sell order round down to the nearest tick"},{"2","More passive - on a buy order round down to the nearest tick; on a sell order round up to the nearest tick"},
	};
msgValueMap[840] = new Dictionary<string, string>()
        {
            {"1","Local (Exchange, ECN, ATS)"},{"2","National"},{"3","Global"},{"4","National excluding local"},
	};
msgValueMap[841] = new Dictionary<string, string>()
        {
            {"0","Floating (default)"},{"1","Fixed"},
	};
msgValueMap[842] = new Dictionary<string, string>()
        {
            {"0","Price (default)"},{"1","Basis Points"},{"2","Ticks"},{"3","Price Tier / Level"},
	};
msgValueMap[843] = new Dictionary<string, string>()
        {
            {"0","Or better (default) - price improvement allowed"},{"1","Strict - limit is a strict limit"},{"2","Or worse - for a buy the discretion price is a minimum and for a sell the discretion price is a maximum (for use for orderswhich have a price range)"},
	};
msgValueMap[844] = new Dictionary<string, string>()
        {
            {"1","More aggressive - on a buy order round the price up to the nearest tick; on a sell round down to the nearest tick"},{"2","More passive - on a buy order round down to the nearest tick; on a sell order round up to the nearest tick"},
	};
msgValueMap[846] = new Dictionary<string, string>()
        {
            {"1","Local (Exchange, ECN, ATS)"},{"2","National"},{"3","Global"},{"4","National excluding local"},
	};

msgValueMap[851] = new Dictionary<string, string>()
        {
            {"1","Added Liquidity"},{"2","Removed Liquidity"},{"3","Liquidity Routed Out"},{"4","Auction"},
	};
msgValueMap[852] = new Dictionary<string, string>()
        {
            {"N","Do Not Report Trade"},{"Y","Report Trade"},
	};
msgValueMap[853] = new Dictionary<string, string>()
        {
            {"0","Dealer Sold Short"},{"1","Dealer Sold Short Exempt"},{"2","Selling Customer Sold Short"},{"3","Selling Customer Sold Short Exempt"},{"4","Qualified Service Representative (QSR) or Automatic Give-up (AGU) Contra Side Sold Short"},{"5","QSR or AGU Contra Side Sold Short Exempt"},
	};
msgValueMap[854] = new Dictionary<string, string>()
        {
            {"0","Units (shares, par, currency)"},{"1","Contracts (if used - must specify ContractMultiplier (tag 231))"},{"2","Units of Measure per Time Unit (if used - must specify UnitofMeasure (tag 996) and TimeUnit (tag 997))"},
	};
msgValueMap[855] = new Dictionary<string, string>()
        {
            {"0","Regular Trade"},{"1","Block Trade"},{"2","EFP (Exchange for physical)"},{"3","Transfer"},{"4","Late Trade"},{"5","T Trade"},{"6","Weighted Average Price Trade"},{"7","Bunched Trade"},{"8","Late Bunched Trade"},{"9","Prior Reference Price Trade"},{"10","After Hours Trade"},{"11","Exchange for Risk (EFR)"},{"12","Exchange for Swap (EFS )"},{"13","Exchange of Futures for (in Market) Futures (EFM ) (e,g, full sized for mini)"},{"14","Exchange of Options for Options (EOO)"},{"15","Trading at Settlement"},{"16","All or None"},{"17","Futures Large Order Execution"},{"18","Exchange of Futures for Futures (external market) (EFF)"},{"19","Option Interim Trade"},{"20","Option Cabinet Trade"},{"22","Privately Negotiated Trades"},{"23","Substitution of Futures for Forwards"},{"24","Error trade"},{"25","Special cum dividend (CD)"},{"26","Special ex dividend (XD)"},{"27","Special cum coupon (CC)"},{"28","Special ex coupon (XC)"},{"29","Cash settlement (CS)"},{"30","Special price (usually net- or all-in price) (SP)"},{"31","Guaranteed delivery (GD)"},{"32","Special cum rights (CR)"},{"33","Special ex rights (XR)"},{"34","Special cum capital repayments (CP)"},{"35","Special ex capital repayments (XP)"},{"36","Special cum bonus (CB)"},{"37","Special ex bonus (XB)"},{"38","Block trade (same as large trade)"},{"39","Worked principal trade (UK-specific)"},{"40","Block Trades - after market"},{"41","Name change"},{"42","Portfolio transfer"},{"43","Prorogation buy - Euronext Paris only. Is used to defer settlement under French SRD (deferred settlement system). Trades must be reported as crosses at zero price"},{"44","Prorogation sell  - see prorogation buy"},{"45","Option exercise"},{"46","Delta neutral transaction"},{"47","Financing transaction(includes repo and stock lending)"},{"48","Non-standard settlement"},{"49","Derivative Related    Transaction"},{"50","Portfolio Trade"},{"51","Volume Weighted Average Trade"},{"52","Exchange Granted Trade"},{"53","Repurchase Agreement"},{"54","OTC"},{"55","Exchange Basis Facility(EBF)"},
	};
msgValueMap[856] = new Dictionary<string, string>()
        {
            {"0","Submit"},{"1","Alleged"},{"2","Accept"},{"3","Decline"},{"4","Addendum"},{"5","No/Was"},{"6","Trade Report Cancel"},{"7","(Locked-In) Trade Break"},{"8","Defaulted"},{"9","Invalid CMTA"},{"10","Pended"},{"11","Alleged New"},{"12","Alleged Addendum"},{"13","Alleged No/Was"},{"14","Alleged Trade Report Cancel"},{"15","Alleged (Locked-In) Trade Break"},
	};
msgValueMap[857] = new Dictionary<string, string>()
        {
            {"0","Not Specified"},{"1","Explicit List Provided"},
	};
msgValueMap[865] = new Dictionary<string, string>()
        {
            {"1","Put"},{"2","Call"},{"3","Tender"},{"4","Sinking Fund Call"},{"5","Activation"},{"6","Inactiviation"},{"7","Last Eligible Trade Date"},{"8","Swap Start Date"},{"9","Swap End Date"},{"10","Swap Roll Date"},{"11","Swap Next Start Date"},{"12","Swap Next Roll Date"},{"13","First Delivery Date"},{"14","Last Delivery Date"},{"15","Initial Inventory Due Date"},{"16","Final Inventory Due Date"},{"17","First Intent Date"},{"18","Last Intent Date"},{"19","Position Removal Date"},{"99","Other"},
	};
msgValueMap[871] = new Dictionary<string, string>()
        {
            {"1","Flat (securities pay interest on a current basis but are traded without interest)"},{"2","Zero coupon"},{"3","Interest bearing (for Euro commercial paper when not issued at discount)"},{"4","No periodic payments"},{"5","Variable rate"},{"6","Less fee for put"},{"7","Stepped coupon"},{"8","Coupon period (if not semi-annual). Supply redemption date in the InstrAttribValue <872>"},{"9","When [and if] issued"},{"10","Original issue discount"},{"11","Callable, puttable"},{"12","Escrowed to Maturity"},{"13","Escrowed to redemption date - callable. Supply redemption date in the InstrAttribValue <872>"},{"14","Pre-refunded"},{"15","In default"},{"16","Unrated"},{"17","Taxable"},{"18","Indexed"},{"19","Subject To Alternative Minimum Tax"},{"20","Original issue discount price. Supply price in the InstrAttribValue <872>"},{"21","Callable below maturity value"},{"22","Callable without notice by mail to holder unless registered"},{"23","Price tick rules for security."},{"24","Trade type eligibility details for security."},{"25","Instrument Denominator"},{"26","Instrument Numerator"},{"27","Instrument Price Precision"},{"28","Instrument Strike Price"},{"29","Tradeable Indicator"},{"99","Text. Supply the text of the attribute or disclaimer in the InstrAttribValue <872>"},
	};
msgValueMap[875] = new Dictionary<string, string>()
        {
            {"1","3(a)(3)"},{"2","4(2)"},{"99","Other"},
	};
msgValueMap[891] = new Dictionary<string, string>()
        {
            {"0","Absolute"},{"1","Per Unit"},{"2","Percentage"},
	};
msgValueMap[893] = new Dictionary<string, string>()
        {
            {"N","Not Last Message"},{"Y","Last Message"},
	};
msgValueMap[895] = new Dictionary<string, string>()
        {
            {"0","Initial"},{"1","Scheduled"},{"2","Time Warning"},{"3","Margin Deficiency"},{"4","Margin Excess"},{"5","Forward Collateral Demand"},{"6","Event of default"},{"7","Adverse tax event"},
	};
msgValueMap[896] = new Dictionary<string, string>()
        {
            {"0","Trade Date"},{"1","GC Instrument"},{"2","Collateral Instrument"},{"3","Substitution Eligible"},{"4","Not Assigned"},{"5","Partially Assigned"},{"6","Fully Assigned"},{"7","Outstanding Trades (Today < end date)"},
	};
msgValueMap[903] = new Dictionary<string, string>()
        {
            {"0","New"},{"1","Replace"},{"2","Cancel"},{"3","Release"},{"4","Reverse"},
	};
msgValueMap[905] = new Dictionary<string, string>()
        {
            {"0","Received"},{"1","Accepted"},{"2","Declined"},{"3","Rejected"},
	};
msgValueMap[906] = new Dictionary<string, string>()
        {
            {"0","Unknown deal (order / trade)"},{"1","Unknown or invalid instrument"},{"2","Unauthorized transaction"},{"3","Insufficient collateral"},{"4","Invalid type of collateral"},{"5","Excessive substitution"},{"99","Other"},
	};
msgValueMap[910] = new Dictionary<string, string>()
        {
            {"0","Unassigned"},{"1","Partially Assigned"},{"2","Assignment Proposed"},{"3","Assigned (Accepted)"},{"4","Challenged"},
	};
msgValueMap[912] = new Dictionary<string, string>()
        {
            {"N","Not last message"},{"Y","Last message"},
	};
msgValueMap[919] = new Dictionary<string, string>()
        {
            {"0","Versus Payment: Deliver (if sell) or Receive (if buy) vs. (against) Payment"},{"1","Free: Deliver (if sell) or Receive (if buy) Free"},{"2","Tri-Party"},{"3","Hold In Custody"},
	};
msgValueMap[924] = new Dictionary<string, string>()
        {
            {"1","Log On User"},{"2","Log Off User"},{"3","Change Password For User"},{"4","Request Individual User Status"},
	};
msgValueMap[926] = new Dictionary<string, string>()
        {
            {"1","Logged In"},{"2","Not Logged In"},{"3","User Not Recognised"},{"4","Password Incorrect"},{"5","Password Changed"},{"6","Other"},{"7","Forced user logout by Exchange"},{"8","Session shutdown warning"},
	};
msgValueMap[928] = new Dictionary<string, string>()
        {
            {"1","Connected"},{"2","Not Connected - down expected up"},{"3","Not Connected - down expected down"},{"4","In Process"},
	};

msgValueMap[937] = new Dictionary<string, string>()
        {
            {"1","Full"},{"2","Incremental Update"},
	};
msgValueMap[939] = new Dictionary<string, string>()
        {
            {"0","Accepted"},{"1","Rejected"},{"3","Accepted with errors"},
	};
msgValueMap[940] = new Dictionary<string, string>()
        {
            {"1","Received"},{"2","Confirm rejected, i.e. not affirmed"},{"3","Affirmed"},
	};
msgValueMap[944] = new Dictionary<string, string>()
        {
            {"0","Retain"},{"1","Add"},{"2","Remove"},
	};
msgValueMap[945] = new Dictionary<string, string>()
        {
            {"0","Accepted"},{"1","Accepted With Warnings"},{"2","Completed"},{"3","Completed With Warnings"},{"4","Rejected"},
	};
msgValueMap[946] = new Dictionary<string, string>()
        {
            {"0","Successful (default)"},{"1","Invalid or unknown instrument"},{"2","Invalid or unknown collateral type"},{"3","Invalid Parties"},{"4","Invalid Transport Type requested"},{"5","Invalid Destination requested"},{"6","No collateral found for the trade specified"},{"7","No collateral found for the order specified"},{"8","Collateral inquiry type not supported"},{"9","Unauthorized for collateral inquiry"},{"99","Other (further information in Text (58) field)"},
	};
msgValueMap[959] = new Dictionary<string, string>()
        {
            {"1","Int"},{"2","Length"},{"3","NumInGroup"},{"4","SeqNum"},{"5","TagNum"},{"6","float"},{"7","Qty"},{"8","Price"},{"9","PriceOffset"},{"10","Amt"},{"11","Percentage"},{"12","Char"},{"13","Boolean"},{"14","String"},{"15","MultipleCharValue"},{"16","Currency"},{"17","Exchange"},{"18","MonthYear"},{"19","UTCTimestamp"},{"20","UTCTimeOnly"},{"21","LocalMktDate"},{"22","UTCDateOnly"},{"23","data"},{"24","MultipleStringValue"},{"25","Country"},{"26","Language"},{"27","TZTimeOnly"},{"28","TZTimestamp"},{"29","Tenor"},
	};
msgValueMap[965] = new Dictionary<string, string>()
        {
            {"1","Active"},{"2","Inactive"},
	};
msgValueMap[974] = new Dictionary<string, string>()
        {
            {"FIXED","FIXED"},{"DIFF","DIFF"},
	};
msgValueMap[975] = new Dictionary<string, string>()
        {
            {"2","T+1"},{"4","T+3"},{"5","T+4"},
	};
msgValueMap[980] = new Dictionary<string, string>()
        {
            {"A","Add"},{"D","Delete"},{"M","Modify"},
	};
msgValueMap[982] = new Dictionary<string, string>()
        {
            {"1","Auto Exercise"},{"2","Non Auto Exercise"},{"3","Final Will Be Exercised"},{"4","Contrary Intention"},{"5","Difference"},
	};
msgValueMap[992] = new Dictionary<string, string>()
        {
            {"1","Sub Allocate"},{"2","Third Party Allocation"},
	};
msgValueMap[996] = new Dictionary<string, string>()
        {
            {"MWh","Megawatt hours"},{"MMBtu","One Million BTU"},{"Bbl","Barrels"},{"Gal","Gallons"},{"t","Metric Tons (aka Tonne)"},{"tn","Tons (US)"},{"MMbbl","Million Barrels"},{"lbs","pounds"},{"oz_tr","Troy Ounces"},{"USD","US Dollars"},{"Bcf","Billion cubic feet"},{"Bu","Bushels"},{"Alw","Allowances"},
	};
msgValueMap[997] = new Dictionary<string, string>()
        {
            {"S","Second"},{"Min","Minute"},{"H","Hour"},{"D","Day"},{"Wk","Week"},{"Mo","Month"},{"Yr","Year"},
	};
msgValueMap[1002] = new Dictionary<string, string>()
        {
            {"1","Automatic"},{"2","Guarantor"},{"3","Manual"},
	};
msgValueMap[1013] = new Dictionary<string, string>()
        {
            {"1","Execution Time"},{"2","Time In"},{"3","Time Out"},{"4","Broker Receipt"},{"5","Broker Execution"},{"6","Desk Receipt"},{"7","Submission to Clearing"},
	};
msgValueMap[1015] = new Dictionary<string, string>()
        {
            {"0","false - trade is not an AsOf trade"},{"1","true - trade is an AsOf  trade"},
	};
msgValueMap[1021] = new Dictionary<string, string>()
        {
            {"1","Top of Book"},{"2","Price Depth"},{"3","Order Depth"},
	};
msgValueMap[1024] = new Dictionary<string, string>()
        {
            {"0","Book"},{"1","Off-Book"},{"2","Cross"},
	};
msgValueMap[1031] = new Dictionary<string, string>()
        {
            {"ADD","Add-on Order"},{"AON","All or None"},{"CNH","Cash Not Held"},{"DIR","Directed Order"},{"E.W","Exchange for Physical Transaction"},{"FOK","Fill or Kill"},{"IO","Imbalance Only"},{"IOC","Immediate or Cancel"},{"LOO","Limit On Open"},{"LOC","Limit on Close"},{"MAO","Market at Open"},{"MAC","Market at Close"},{"MOO","Market on Open"},{"MOC","Market On Close"},{"MQT","Minimum Quantity"},{"NH","Not Held"},{"OVD","Over the Day"},{"PEG","Pegged"},{"RSV","Reserve Size Order"},{"S.W","Stop Stock Transaction"},{"SCL","Scale"},{"TMO","Time Order"},{"TS","Trailing Stop"},{"WRK","Work"},
	};
msgValueMap[1032] = new Dictionary<string, string>()
        {
            {"1","NASD OATS"},
	};
msgValueMap[1033] = new Dictionary<string, string>()
        {
            {"A","Agency"},{"AR","Arbitrage"},{"D","Derivatives"},{"IN","International"},{"IS","Institutional"},{"O","Other"},{"PF","Preferred Trading"},{"PR","Proprietary"},{"PT","Program Trading"},{"S","Sales"},{"T","Trading"},
	};
msgValueMap[1034] = new Dictionary<string, string>()
        {
            {"1","NASD OATS"},
	};
msgValueMap[1035] = new Dictionary<string, string>()
        {
            {"ADD","Add-on Order"},{"AON","All or None"},{"CNH","Cash Not Held"},{"DIR","Directed Order"},{"E.W","Exchange for Physical Transaction"},{"FOK","Fill or Kill"},{"IO","Imbalance Only"},{"IOC","Immediate or Cancel"},{"LOO","Limit On Open"},{"LOC","Limit on Close"},{"MAO","Market at Open"},{"MAC","Market at Close"},{"MOO","Market on Open"},{"MOC","Market On Close"},{"MQT","Minimum Quantity"},{"NH","Not Held"},{"OVD","Over the Day"},{"PEG","Pegged"},{"RSV","Reserve Size Order"},{"S.W","Stop Stock Transaction"},{"SCL","Scale"},{"TMO","Time Order"},{"TS","Trailing Stop"},{"WRK","Work"},
	};
msgValueMap[1036] = new Dictionary<string, string>()
        {
            {"0","Received, not yet processed"},{"1","Accepted"},{"2","Don't know / Rejected"},
	};
msgValueMap[1043] = new Dictionary<string, string>()
        {
            {"0","Specific Deposit"},{"1","General"},
	};
msgValueMap[1046] = new Dictionary<string, string>()
        {
            {"M","Multiply"},{"D","Divide"},
	};
msgValueMap[1047] = new Dictionary<string, string>()
        {
            {"O","Open"},{"C","Close"},{"R","Rolled"},{"F","FIFO"},
	};
msgValueMap[1048] = new Dictionary<string, string>()
        {
            {"A","Agent"},{"P","Principal"},{"R","Riskless Principal"},
	};
msgValueMap[1049] = new Dictionary<string, string>()
        {
            {"					R","Random"},{"					P","ProRata"},
	};
msgValueMap[1057] = new Dictionary<string, string>()
        {
            {"Y","Order initiator is aggressor"},{"N","Order initiator is passive"},
	};
msgValueMap[1070] = new Dictionary<string, string>()
        {
            {"0","Indicative"},{"1","Tradeable"},{"2","Restricted Tradeable"},{"3","Counter"},{"4","Indicative and Tradeable"},
	};
msgValueMap[1081] = new Dictionary<string, string>()
        {
            {"0","SecondaryOrderID(198)"},{"1","OrderID(37)"},{"2","MDEntryID(278)"},{"3","QuoteEntryID(299)"},{"4","Original order ID"},
	};
msgValueMap[1083] = new Dictionary<string, string>()
        {
            {"1","Immediate"},{"2","Exhaust"},
	};
msgValueMap[1084] = new Dictionary<string, string>()
        {
            {"1","Initial"},{"2","New"},{"3","Random"},{"4","Undisclosed (invisible order)"},
	};
msgValueMap[1092] = new Dictionary<string, string>()
        {
            {"0","None"},{"1","Local (Exchange, ECN, ATS)"},{"2","National (Across all national markets)"},{"3","Global (Across all markets)"},
	};
msgValueMap[1093] = new Dictionary<string, string>()
        {
            {"1","Odd Lot"},{"2","Round Lot"},{"3","Block Lot"},{"4","Round lot based upon UnitOfMeasure <996>"},
	};
msgValueMap[1094] = new Dictionary<string, string>()
        {
            {"1","Last peg"},{"2","Mid-price peg"},{"3","Opening peg"},{"4","Market peg"},{"5","Primary peg"},{"7","Peg to VWAP"},{"8","Trailing Stop Peg"},{"9","Peg to Limit Price"},
	};
msgValueMap[1100] = new Dictionary<string, string>()
        {
            {"1","Partial Execution"},{"2","Specified Trading Session"},{"3","Next Auction"},{"4","Price Movement"},
	};
msgValueMap[1101] = new Dictionary<string, string>()
        {
            {"1","Activate"},{"2","Modify"},{"3","Cancel"},
	};
msgValueMap[1107] = new Dictionary<string, string>()
        {
            {"1","Best Offer"},{"2","Last Trade"},{"3","Best Bid"},{"4","Best Bid or Last Trade"},{"5","Best Offer or Last Trade"},{"6","Best Mid"},
	};
msgValueMap[1108] = new Dictionary<string, string>()
        {
            {"0","None"},{"1","Local (Exchange, ECN, ATS)"},{"2","National (Across all national markets)"},{"3","Global (Across all markets)"},
	};
msgValueMap[1109] = new Dictionary<string, string>()
        {
            {"U","Trigger if the price of the specified type goes UP to or through the specified Trigger Price."},{"D","Trigger if the price of the specified type goes DOWN to or through the specified Trigger Price."},
	};
msgValueMap[1111] = new Dictionary<string, string>()
        {
            {"1","Market"},{"2","Limit"},
	};
msgValueMap[1115] = new Dictionary<string, string>()
        {
            {"1","Order"},{"2","Quote"},{"3","Privately Negotiated Trade"},{"4","Multileg order"},{"5","Linked order"},{"6","Quote Request"},{"7","Implied Order"},{"8","Cross Order"},{"9","Streaming price (quote)"},
	};
msgValueMap[1123] = new Dictionary<string, string>()
        {
            {"0","Trade Confirmation"},{"1","Two-Party Report"},{"2","One-Party Report for Matching"},{"3","One-Party Report for Pass Through"},{"4","Automated Floor Order Routing"},{"5","Two Party Report for Claim"},
	};
msgValueMap[1128] = new Dictionary<string, string>()
        {
            {"0","FIX27"},{"1","FIX30"},{"2","FIX40"},{"3","FIX41"},{"4","FIX42"},{"5","FIX43"},{"6","FIX44"},{"7","FIX50"},{"8","FIX50SP1"},{"9","FIX50SP2"},
	};
msgValueMap[1133] = new Dictionary<string, string>()
        {
            {"B","BIC (Bank Identification Code) (ISO 9362)"},{"C","Generally accepted market participant identifier (e.g. NASD mnemonic)"},{"D","Proprietary / Custom code"},{"E","ISO Country Code"},{"G","MIC (ISO 10383 - Market Identifier Code)"},
	};
msgValueMap[1144] = new Dictionary<string, string>()
        {
            {"0","Not implied"},{"1","Implied-in - The existence of a multi-leg instrument is implied by the legs of that instrument"},{"2","Implied-out  - The existence of the underlying legs are implied by the multi-leg instrument"},{"3","Both Implied-in and Implied-out"},
	};
msgValueMap[1159] = new Dictionary<string, string>()
        {
            {"1","Preliminary"},{"2","Final"},
	};
msgValueMap[1162] = new Dictionary<string, string>()
        {
            {"Transaction Type - required except where SettlInstMode is ","eject SSI request"},{"C","Cancel"},{"N","New"},{"R","Replace"},{"T","Restate"},
	};
msgValueMap[1164] = new Dictionary<string, string>()
        {
            {"1","Instructions of Broker"},{"2","Instructions for Institution"},{"3","Investor"},
	};
msgValueMap[1167] = new Dictionary<string, string>()
        {
            {"0","Accepted"},{"5","Rejected"},{"6","Removed from Market"},{"7","Expired"},{"12","Locked Market Warning"},{"13","Cross Market Warning"},{"14","Canceled due to Lock Market"},{"15","Canceled due to Cross Market"},{"16","Active"},
	};
msgValueMap[1171] = new Dictionary<string, string>()
        {
            {"            'Y'","Private Quote"},{"            'N'","Public Quote"},
	};
msgValueMap[1172] = new Dictionary<string, string>()
        {
            {"1","All market participants"},{"2","Specified market participants"},{"3","All Market Makers"},{"4","Primary Market Maker(s)"},
	};
msgValueMap[1174] = new Dictionary<string, string>()
        {
            {"1","Order imbalance, auction is extended"},{"2","Trading resumes (after Halt)"},{"3","Price Volatility Interruption"},{"4","Change of Trading Session"},{"5","Change of Trading Subsession"},{"6","Change of Security Trading Status"},{"7","Change of Book Type"},{"8","Change of Market Depth"},
	};
msgValueMap[1176] = new Dictionary<string, string>()
        {
            {"1","Exchange Last"},{"2","High / Low Price"},{"3","Average Price (VWAP, TWAP ... )"},{"4","Turnover (Price * Qty)"},
	};
msgValueMap[1178] = new Dictionary<string, string>()
        {
            {"1","Customer"},
	};
msgValueMap[1193] = new Dictionary<string, string>()
        {
            {"C","Cash settlement required"},{"P","Physical settlement required"},
	};
msgValueMap[1194] = new Dictionary<string, string>()
        {
            {"0","European"},{"1","American"},{"2","Bermuda"},
	};
msgValueMap[1196] = new Dictionary<string, string>()
        {
            {"STD","Standard,  money per unit of a physical"},{"INX","Index"},{"INT","Interest rate Index"},{"PCTPAR","Percent of Par"},
	};
msgValueMap[1197] = new Dictionary<string, string>()
        {
            {"EQTY","premium style"},{"FUT","futures style mark-to-market"},{"FUTDA","futures style with an attached cash adjustment"},{"CDS","CDS style collateralization of market to market and coupon"},{"CDSD","CDS in delivery - use recovery rate to calculate obligation"},
	};
msgValueMap[1198] = new Dictionary<string, string>()
        {
            {"0","pre-listed only"},{"1","user requested"},
	};
msgValueMap[1209] = new Dictionary<string, string>()
        {
            {"0","Regular"},{"1","Variable"},{"2","Fixed"},{"3","Traded as a spread leg"},{"4","Settled as a spread leg"},
	};
msgValueMap[1302] = new Dictionary<string, string>()
        {
            {"0","Months"},{"1","Days"},{"2","Weeks"},{"3","Years"},
	};
msgValueMap[1303] = new Dictionary<string, string>()
        {
            {"0","YearMonth Only (default)"},{"1","YearMonthDay"},{"2","YearMonthWeek"},
	};
msgValueMap[1306] = new Dictionary<string, string>()
        {
            {"0","Price"},{"1","Ticks"},{"2","Percentage"},
	};
msgValueMap[1307] = new Dictionary<string, string>()
        {
            {"0","Symbol"},{"1","SecurityType and or CFICode"},{"2","Product"},{"3","TradingSessionID"},{"4","All Securities"},{"5","UndelyingSymbol"},{"6","Underlying SecurityType and or CFICode"},{"7","Underlying Product"},{"8","MarketID or MarketID + MarketSegmentID"},
	};
msgValueMap[1347] = new Dictionary<string, string>()
        {
            {"0","Retransmission of application messages for the specified Applications"},{"1","Subscription to the specified Applications"},{"2","Request for the last ApplLastSeqNum published for the specified Applications"},{"3","Request valid set of Applications"},{"4","Unsubscribe to the specified Applications"},{"5","Cancel retransmission"},{"6","Cancel retransmission and unsubscribe to the specified applications"},
	};
msgValueMap[1348] = new Dictionary<string, string>()
        {
            {"0","Request successfully processed"},{"1","Application does not exist"},{"2","Messages not available"},
	};
msgValueMap[1354] = new Dictionary<string, string>()
        {
            {"0","Application does not exist"},{"1","Messages requested are not available"},{"2","User not authorized for application"},
	};
msgValueMap[1368] = new Dictionary<string, string>()
        {
            {"0","Trading resumes (after Halt)"},{"1","Change of Trading Session"},{"2","Change of Trading Subsession"},{"3","Change of Trading Status"},
	};
msgValueMap[1373] = new Dictionary<string, string>()
        {
            {"1","Suspend orders"},{"2","Release orders from suspension"},{"3","Cancel orders"},
	};
msgValueMap[1374] = new Dictionary<string, string>()
        {
            {"1","All orders for a security"},{"2","All orders for an underlying security"},{"3","All orders for a Product"},{"4","All orders for a CFICode"},{"5","All orders for a SecurityType"},{"6","All orders for a trading session"},{"7","All orders"},{"8","All orders for a Market"},{"9","All orders for a Market Segment"},{"10","All orders for a Security Group"},{"11","Cancel for Security Issuer"},{"12","Cancel for Issuer of Underlying Security"},
	};
msgValueMap[1375] = new Dictionary<string, string>()
        {
            {"0","Rejected - See MassActionRejectReason(1376)"},{"1","Accepted"},
	};
msgValueMap[1376] = new Dictionary<string, string>()
        {
            {"0","Mass Action Not Supported"},{"1","Invalid or unknown security"},{"2","Invalid or unknown underlying security"},{"3","Invalid or unknown Product"},{"4","Invalid or unknown CFICode"},{"5","Invalid or unknown SecurityType"},{"6","Invalid or unknown trading session"},{"7","Invalid or unknown Market"},{"8","Invalid or unknown Market Segment"},{"9","Invalid or unknown Security Group"},{"99","Other"},{"10","Invalid or unknown Security Issuer"},{"11","Invalid or unknown Issuer of Underlying Security "},
	};
msgValueMap[1377] = new Dictionary<string, string>()
        {
            {"0","Predefined Multileg Security"},{"1","User-defined Multleg Security"},{"2","User-defined, Non-Securitized, Multileg"},
	};
msgValueMap[1378] = new Dictionary<string, string>()
        {
            {"0","Net Price"},{"1","Reversed Net Price"},{"2","Yield Difference"},{"3","Individual"},{"4","Contract Weighted Average Price"},{"5","Multiplied Price"},
	};
msgValueMap[1385] = new Dictionary<string, string>()
        {
            {"1","One Cancels the Other (OCO)"},{"2","One Triggers the Other (OTO)"},{"3","One Updates the Other (OUO) - Absolute Quantity Reduction"},{"4","One Updates the Other (OUO) - Proportional Quantity Reduction"},
	};
msgValueMap[1386] = new Dictionary<string, string>()
        {
            {"0","Broker / Exchange option"},{"2","Exchange closed"},{"4","Too late to enter"},{"5","Unknown order"},{"6","Duplicate Order (e.g. dupe ClOrdID)"},{"11","Unsupported order characteristic"},{"99","Other"},
	};
msgValueMap[1390] = new Dictionary<string, string>()
        {
            {"0","Do Not Publish Trade"},{"1","Publish Trade"},{"2","Deferred Publication"},
	};
msgValueMap[1395] = new Dictionary<string, string>()
        {
            {"A","Add"},{"D","Delete"},{"M","Modify"},
	};
msgValueMap[1409] = new Dictionary<string, string>()
        {
            {"0","Session active"},{"1","Session password changed"},{"2","Session password due to expire"},{"3","New session password does not comply with policy"},{"4","Session logout complete"},{"5","Invalid username or password"},{"6","Account locked"},{"7","Logons are not allowed at this time"},{"8","Password expired"},
	};
msgValueMap[1426] = new Dictionary<string, string>()
        {
            {"0","Reset ApplSeqNum to new value specified in ApplNewSeqNum(1399)"},{"1","Reports that the last message has been sent for the ApplIDs Refer to RefApplLastSeqNum(1357) for the application sequencenumber of the last message."},{"2","Heartbeat message indicating that Application identified by RefApplID(1355) is still alive.Refer to RefApplLastSeqNum(1357) for the application sequence number of the previous message."},{"3","Application message re-send completed."},
	};
msgValueMap[1429] = new Dictionary<string, string>()
        {
            {"0","Seconds (default if not specified)"},{"1","Tenths of a second"},{"2","Hundredths of a second"},{"3","milliseconds"},{"4","microseconds"},{"5","nanoseconds"},{"10","minutes"},{"11","hours"},{"12","days"},{"13","weeks"},{"14","months"},{"15","years"},
	};
msgValueMap[1430] = new Dictionary<string, string>()
        {
            {"E","Electronic"},{"P","Pit"},{"X","Ex-Pit"},
	};
msgValueMap[1431] = new Dictionary<string, string>()
        {
            {"0","GTC from previous day"},{"1","Partial Fill Remaining"},{"2","Order Changed"},
	};
msgValueMap[1432] = new Dictionary<string, string>()
        {
            {"1","Member trading for their own account"},{"2","Clearing Firm trading for its proprietary account"},{"3","Member trading for another member"},{"4","All other"},
	};
msgValueMap[1434] = new Dictionary<string, string>()
        {
            {"0","Utility provided standard model"},{"1","Proprietary (user supplied) model"},
	};
msgValueMap[1435] = new Dictionary<string, string>()
        {
            {"0","Shares"},{"1","Hours"},{"2","Days"},
	};
msgValueMap[1439] = new Dictionary<string, string>()
        {
            {"0","NERC Eastern Off-Peak"},{"1","NERC Western Off-Peak"},{"2","NERC Calendar-All Days in month"},{"3","NERC Eastern Peak"},{"4","NERC Western Peak"},
	};
msgValueMap[1446] = new Dictionary<string, string>()
        {
            {"0","Bloomberg"},{"1","Reuters"},{"2","Telerate"},{"99","Other"},
	};
msgValueMap[1447] = new Dictionary<string, string>()
        {
            {"0","Primary"},{"1","Secondary"},
	};
msgValueMap[1449] = new Dictionary<string, string>()
        {
            {"FR","Full Restructuring"},{"MR","Modified Restructuring"},{"MM","Modified Mod Restructuring"},{"XR","No Restructuring specified"},
	};
msgValueMap[1450] = new Dictionary<string, string>()
        {
            {"SD","Senior Secured"},{"SR","Senior"},{"SB","Subordinated"},
	};
msgValueMap[1470] = new Dictionary<string, string>()
        {
            {"1","Industry Classification"},{"2","Trading List"},{"3","Market / Market Segment List"},{"4","Newspaper List"},
	};
msgValueMap[1471] = new Dictionary<string, string>()
        {
            {"1","ICB (Industry Classification Benchmark) published by Dow Jones and FTSE - www.icbenchmark.com"},{"2","NAICS (North American Industry Classification System). Replaced SIC (Standard Industry Classification) www.census.gov/naics or www.naics.com. "},{"3","GICS (Global Industry Classification Standard) published by Standards &amp; Poor"},
	};
msgValueMap[1473] = new Dictionary<string, string>()
        {
            {"0","Company News"},{"1","Marketplace News"},{"2","Financial Market News"},{"3","Technical News"},{"99","Other News"},
	};
msgValueMap[1477] = new Dictionary<string, string>()
        {
            {"0","Replacement"},{"1","Other Language"},{"2","Complimentary"},
	};
msgValueMap[1478] = new Dictionary<string, string>()
        {
            {"1","Fixed Strike"},{"2","Strike set at expiration to underlying or other value (lookback floating)"},{"3","Strike set to average of underlying settlement price across the life of the option"},{"4","Strike set to optimal value"},
	};
msgValueMap[1479] = new Dictionary<string, string>()
        {
            {"1","Less than underlying price is in-the-money (ITM)"},{"2","Less than or equal to the underlying price is in-the-money(ITM)"},{"3","Equal to the underlying price is in-the-money(ITM)"},{"4","Greater than or equal to underlying price is in-the-money(ITM)"},{"5","Greater than underlying is in-the-money(ITM)"},
	};
msgValueMap[1481] = new Dictionary<string, string>()
        {
            {"1","Regular"},{"2","Special reference"},{"3","Optimal value (Lookback)"},{"4","Average value (Asian option)"},
	};
msgValueMap[1482] = new Dictionary<string, string>()
        {
            {"1","Vanilla"},{"2","Capped"},{"3","Binary"},
	};
msgValueMap[1484] = new Dictionary<string, string>()
        {
            {"1","Capped"},{"2","Trigger"},{"3","Knock-in up"},{"4","Kock-in down"},{"5","Knock-out up"},{"6","Knock-out down"},{"7","Underlying"},{"8","Reset Barrier"},{"9","Rolling Barrier"},
	};
msgValueMap[1487] = new Dictionary<string, string>()
        {
            {"1","Less than ComplexEventPrice(1486)"},{"2","Less than or equal to ComplexEventPrice(1486)"},{"3","Equal to ComplexEventPrice(1486)"},{"4","Greater than or equal to ComplexEventPrice(1486)"},{"5","Greater than ComplexEventPrice(1486)"},
	};
msgValueMap[1489] = new Dictionary<string, string>()
        {
            {"1","Expiration"},{"2","Immediate (At Any Time)"},{"3","Specified Date/Time"},
	};
msgValueMap[1490] = new Dictionary<string, string>()
        {
            {"1","And"},{"2","Or"},
	};
msgValueMap[1498] = new Dictionary<string, string>()
        {
            {"1","Stream assignment for new customer(s)"},{"2","Stream assignment for existing customer(s)"},
	};
msgValueMap[1502] = new Dictionary<string, string>()
        {
            {"0","Unknown client"},{"1","Exceeds maximum size"},{"2","Unknown or Invalid currency pair"},{"3","No available stream"},{"99","Other"},
	};
msgValueMap[1503] = new Dictionary<string, string>()
        {
            {"0","Assignment Accepted"},{"1","Assignment Rejected"},
	};
msgValueMap[1507] = new Dictionary<string, string>()
        {
            {"0","Return all available information on parties and related parties. (May not be combined with other values.)"},{"1","Return only party information. (Excludes information like risk limits from being reported. May not be combined with any other value except 2.)"},{"2","Include information on related parties."},{"3","Include risk limit information."},
	};
msgValueMap[1511] = new Dictionary<string, string>()
        {
            {"0","Valid request"},{"1","Invalid or unsupported request"},{"2","No parties or party details found that match selection criteria"},{"3","Unsupported PartyListResponseType"},{"4","Not authorized to retrieve parties or party details data"},{"5","Parties or party details data temporarily unavailable"},{"6","Request for parties data not supported"},{"99","Other (further information in Text (58) field)"},
	};
msgValueMap[1515] = new Dictionary<string, string>()
        {
            {"0","Is also"},{"1","Clears for"},{"2","Clears through"},{"3","Trades for"},{"4","Trades through"},{"5","Sponsors"},{"6","Sponsored through"},{"7","Provides guarantee for"},{"8","Is guaranteed by"},{"9","Member of"},{"10","Has members"},{"11","Provides marketplace for"},{"12","Participant of marketplace"},{"13","Carries positions for"},{"14","Posts trades to"},{"15","Enters trades for"},{"16","Enters trades through"},{"17","Provides quotes to"},{"18","Requests quotes from"},{"19","Invests for"},{"20","Invests through"},{"21","Brokers trades for"},{"22","Brokers trades through"},{"23","Provides trading services for"},{"24","Uses trading services of"},{"25","Approves of"},{"26","Approved by"},{"27","Parent firm for"},{"28","Subsidiary of"},{"29","Regulatory owner of"},{"30","Owned by (regulatory)"},{"31","Controls"},{"32","Is controlled by"},{"33","Legal / titled owner of"},{"34","Owned by (legal / title)"},{"35","Beneficial owner of"},{"36","Owned by (beneficial)"},
	};
msgValueMap[1530] = new Dictionary<string, string>()
        {
            {"1","Gross Limit"},{"2","Net Limit"},{"3","Exposure"},{"4","Long Limit"},{"5","Short Limit"},
	};
msgValueMap[1535] = new Dictionary<string, string>()
        {
            {"1","Include"},{"2","Exclude"},
	};
msgValueMap[1617] = new Dictionary<string, string>()
        {
            {"1","Assignment"},{"2","Rejected"},{"3","Terminate/Unassign"},
	};


        }
    }
}
