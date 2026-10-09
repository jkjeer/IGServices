<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:ms="urn:schemas-microsoft-com:xslt">

  <xsl:template match="root">
    <html>
      <head>
        <style>
          body,td,p,th  { font-family: arial; font-size: 8pt; }
          tr.head { border : 0px solid black; padding: 0 0px; background: #ddddff; font-weight:bold;}
          td.head { border : 0px solid black; padding: 0 0px; background: #ddddff; }
          td.numeric60 { text-align:right; width:60px;  }
          td.noBorder {border: none; }
          .blank {background: #ffffff;}
          tr.noBorder td {border: 0;}
          .error {color: red;}
          .backGreen {background-color: green; color:white;}
          .backRed {background-color: red; color:white;}
          .backYellow {background-color: yellow; color:black;}
          .title {font-weight:bold; text-decoration: underline;}
          .bold {font-weight:bold;}
          .totals {font-weight:bold;}
          .footerTotals {font-weight:bold;color: white;background: #000000; }
          .depth1 {text-indent: 0px;}
          .depth2 {text-indent: 5px;}
          .depth3 {text-indent: 10px;}
          .depth4 {text-indent: 15px;}
          .depth5 {text-indent: 20px;}
          .depth6 {text-indent: 25px;}
          .depth7 {text-indent: 30px;}
          .depth8 {text-indent: 35px;}
          .depth9 {text-indent: 40px;}
          tr.odd { background: #ffffff; }
          tr.even { background: #D5CFCF; }
          .defaultTable td, table
          {
          border: 1px solid black;
          border-collapse:collapse;
          padding:2px;
          }
          .defaultTable1 table
          {
          border: none;
          }
        </style>
      </head>
      <body>


        <div class="title">Current Release</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Truck Num</td>
            <td align="left">Release Date</td>
            <td align="left">Departure Date</td>
            <td align="left">Arrival Date</td>
          </tr>
          <xsl:apply-templates select="SerializedTruckList/Trucks/Truck[CurrentRelease='Y']">
          </xsl:apply-templates>
        </table>
        <br/>

        <!--
        <div class="title">Summary</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">PlantCode</td>
            <td align="left">Start Date</td>
            <td align="left">End Date</td>
            <td align="right">ElapsedSeconds</td>
          </tr>
          <xsl:apply-templates select="Summary/Summary">
          </xsl:apply-templates>
        </table>
        <br/>
        -->

        <div class="title">Product Lines</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td colspan="8"></td>
            <td colspan="3" align="center">Units</td>
            <td colspan="6" align="center">Racks</td>
          </tr>
          <tr class="head">
            <td align="left">Product Line</td>
            <td align="left">Hrly Rate</td>
            <td align="left">Min Floor</td>
            <td align="left">Max Floor</td>
            <td align="left">Last Ship Date</td>
            <td align="left">Last Consumed Date</td>
            <td align="left">Shift</td>
            <td align="left">Active</td>
            <td align="right">Trgt Units</td>
            <td align="right">Floor Units</td>
            <td align="right">Reqd Units</td>
            <td align="right" width="50px">B26..</td>
            <td align="right" width="50px">B52..</td>
            <td align="right" width="50px">A84..</td>
            <td align="right" width="50px">A110.</td>
            <td align="right" width="50px">IG69.</td>
            <td align="right" width="50px">Other</td>
          </tr>
          <xsl:apply-templates select="SerializedProductLines1/ArrayOfRbaProductLine/RbaProductLine">
            <xsl:sort select="SortIndex" order="ascending" data-type="number" />
          </xsl:apply-templates>
          <tr>
            <td>HOT RACKS</td>
            <td colspan="10"></td>
            <td class="backGreen" align="right">
              <xsl:value-of select="format-number(count(SerializedHotRacks/HotRacks/HotRack[RackType='B26']), '#,###')"/>
            </td>
            <td class="backGreen" align="right">
              <xsl:value-of select="format-number(count(SerializedHotRacks/HotRacks/HotRack[RackType='B52']), '#,###')"/>
            </td>
            <td class="backGreen" align="right">
              <xsl:value-of select="format-number(count(SerializedHotRacks/HotRacks/HotRack[RackType='A84']), '#,###')"/>
            </td>
            <td class="backGreen" align="right">
              <xsl:value-of select="format-number(count(SerializedHotRacks/HotRacks/HotRack[RackType='A110']), '#,###')"/>
            </td>
            <td class="backGreen" align="right">
              <xsl:value-of select="format-number(count(SerializedHotRacks/HotRacks/HotRack[RackType='IG69']), '#,###')"/>
            </td>
            <td class="backGreen" align="right">
              <xsl:value-of select="format-number(count(SerializedHotRacks/HotRacks/HotRack[not(RackType='B26' or RackType='B52' or RackType='A84' or RackType='A110' or RackType='IG69')]), '#,###')"/>
            </td>
          </tr>
          <tr class="head">
            <td colspan="11"></td>
            <td align="right">
              <xsl:value-of select="format-number(count(SerializedProductLines1/ArrayOfRbaProductLine/RbaProductLine/NextRacks/RbaRack[RackType='B26' and IsRequired='Y']) + count(SerializedHotRacks/HotRacks/HotRack[RackType='B26']), '#,##0')"/>
            </td>
            <td align="right">
              <xsl:value-of select="format-number(count(SerializedProductLines1/ArrayOfRbaProductLine/RbaProductLine/NextRacks/RbaRack[RackType='B52' and IsRequired='Y']) + count(SerializedHotRacks/HotRacks/HotRack[RackType='B52']), '#,##0')"/>
            </td>
            <td align="right">
              <xsl:value-of select="format-number(count(SerializedProductLines1/ArrayOfRbaProductLine/RbaProductLine/NextRacks/RbaRack[RackType='A84' and IsRequired='Y']) + count(SerializedHotRacks/HotRacks/HotRack[RackType='A84']), '#,##0')"/>
            </td>
            <td align="right">
              <xsl:value-of select="format-number(count(SerializedProductLines1/ArrayOfRbaProductLine/RbaProductLine/NextRacks/RbaRack[RackType='A110' and IsRequired='Y']) + count(SerializedHotRacks/HotRacks/HotRack[RackType='A110']), '#,##0')"/>
            </td>
            <td align="right">
              <xsl:value-of select="format-number(count(SerializedProductLines1/ArrayOfRbaProductLine/RbaProductLine/NextRacks/RbaRack[RackType='IG69' and IsRequired='Y']) + count(SerializedHotRacks/HotRacks/HotRack[RackType='IG69']), '#,##0')"/>
            </td>
            <td align="right">
              <xsl:value-of select="format-number(count(SerializedProductLines1/ArrayOfRbaProductLine/RbaProductLine/NextRacks/RbaRack[not(RackType='B26' or RackType='B52' or RackType='A84' or RackType='A110' or RackType='IG69') and IsRequired='Y']) + count(SerializedHotRacks/HotRacks/HotRack[not(RackType='B26' or RackType='B52' or RackType='A84' or RackType='A110' or RackType='IG69')]), '#,##0')"/>
            </td>
          </tr>
        </table>
        <br/>

        <xsl:if test="//Error">
          <div class="title">Errors</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Message</td>
              <td align="left">Source</td>
              <td align="left">Stack Trace</td>
            </tr>
            <xsl:apply-templates select="//Error">
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>

        <hr/>
        <br/>
        <div class="title">Hot Racks</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Rack Num</td>
            <td align="left">Schedule Num</td>
            <td align="left">Box Num</td>
            <td align="left">Due Date</td>
            <td align="left">Rack Type</td>
            <td align="left">Status</td>
            <td align="left">Priority</td>
            <td align="left">Units</td>
          </tr>
          <xsl:apply-templates select="SerializedHotRacks/HotRacks/HotRack">
            <xsl:sort select="RackNum" order="ascending" data-type="number" />
          </xsl:apply-templates>
        </table>
        <br/>

        <hr/>
        <br/>
        <div class="title">Next Racks (Top 10)</div>
        <br/>
        <table class="defaultTable1">
          <tr>
            <td>
              <table class="defaultTable1" width="100%">
                <tr>
                  <td>
                    <xsl:apply-templates select="SerializedProductLines2/ArrayOfRbaProductLine/RbaProductLine">
                      <xsl:sort select="SortIndex" order="ascending" data-type="number"/>
                    </xsl:apply-templates>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>
        <br/>

      </body>
    </html>
  </xsl:template>


  <xsl:template match="Truck">
    <tr>
      <td>
        <xsl:value-of select="TruckNum"/>
      </td>
      <td>
        <xsl:value-of select="ReleaseDateString"/>
      </td>
      <td>
        <xsl:value-of select="DepartureDateString"/>
      </td>
      <td>
        <xsl:value-of select="ArrivalDateString"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="Summary">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="PlantCode"/>
      </td>
      <td>
        <xsl:value-of select="StartDate"/>
      </td>
      <td>
        <xsl:value-of select="EndDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="format-number(ElapsedSeconds, '#,##0.000')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="SerializedProductLines1/ArrayOfRbaProductLine/RbaProductLine">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="ShortCode"/>
      </td>
      <td>
        <xsl:value-of select="HourlyRunRate"/>
      </td>
      <td>
        <xsl:value-of select="MinFloorQty"/>
      </td>
      <td>
        <xsl:value-of select="MaxFloorQty"/>
      </td>
      <td>
        <xsl:value-of select="LastShipDateString"/>
      </td>
      <td>
        <xsl:value-of select="LastConsumedDateString"/>
      </td>
      <td>
        <xsl:value-of select="TargetShift"/>
      </td>
      <td>
        <xsl:value-of select="TargetShiftActive"/>
      </td>
      <td align="right">
        <xsl:value-of select="ReorderQty"/>
      </td>
      <td align="right">
        <xsl:value-of select="RbaFloorUnits"/>
      </td>
      <td align="right">
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'Y'">backGreen</xsl:when>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'N'">backYellow</xsl:when>
            <xsl:otherwise>backRed</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:value-of select="RequiredUnits"/>
      </td>
      <td align="right">
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'Y'">backGreen</xsl:when>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'N'">backYellow</xsl:when>
            <xsl:otherwise>backRed</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:value-of select="format-number(count(NextRacks/RbaRack[RackType='B26' and IsRequired='Y']), '#,###')"/>
      </td>
      <td align="right">
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'Y'">backGreen</xsl:when>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'N'">backYellow</xsl:when>
            <xsl:otherwise>backRed</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:value-of select="format-number(count(NextRacks/RbaRack[RackType='B52' and IsRequired='Y']), '#,###')"/>
      </td>
      <td align="right">
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'Y'">backGreen</xsl:when>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'N'">backYellow</xsl:when>
            <xsl:otherwise>backRed</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:value-of select="format-number(count(NextRacks/RbaRack[RackType='A84' and IsRequired='Y']), '#,###')"/>
      </td>
      <td align="right">
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'Y'">backGreen</xsl:when>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'N'">backYellow</xsl:when>
            <xsl:otherwise>backRed</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:value-of select="format-number(count(NextRacks/RbaRack[RackType='A110' and IsRequired='Y']), '#,###')"/>
      </td>
      <td align="right">
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'Y'">backGreen</xsl:when>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'N'">backYellow</xsl:when>
            <xsl:otherwise>backRed</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:value-of select="format-number(count(NextRacks/RbaRack[RackType='IG69' and IsRequired='Y']), '#,###')"/>
      </td>
      <td align="right">
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'Y'">backGreen</xsl:when>
            <xsl:when test="RequiredUnits > 0 and TargetShiftActive = 'N'">backYellow</xsl:when>
            <xsl:otherwise>backRed</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:value-of select="format-number(count(NextRacks/RbaRack[not(RackType='B26' or RackType='B52' or RackType='A84' or RackType='A110' or RackType='IG69') and IsRequired='Y']), '#,###')"/>
      </td>
    </tr>
  </xsl:template>


  <xsl:template match="HotRack">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="RackNum"/>
      </td>
      <td>
        <xsl:value-of select="ScheduleNum"/>
      </td>
      <td>
        <xsl:value-of select="BoxNum"/>
      </td>
      <td>
        <xsl:value-of select="DueDate"/>
      </td>
      <td>
        <xsl:value-of select="RackType"/>
      </td>
      <td>
        <xsl:value-of select="HeaderStatus"/>
      </td>
      <td>
        <xsl:value-of select="PriorityCode"/>
      </td>
      <td align="right">
        <xsl:value-of select="Units"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="SerializedProductLines2/ArrayOfRbaProductLine/RbaProductLine">
    <xsl:if test="ReorderQty > 0">
      <table class="defaultTable1" width="100%">
        <tr>
          <td>
            <table class="defaultTable" width="100%">
              <tr class="head">
                <td align="left">Product Line</td>
                <td align="left">Product Line Name</td>
                <td align="left">Last Ship Date</td>
                <td align="left">Last Consumed Date</td>
                <td align="right">Target Units</td>
                <td align="right">Floor Units</td>
                <td align="right">Required Units</td>
              </tr>
              <tr class="bold">
                <td>
                  <xsl:value-of select="ShortCode"/>
                </td>
                <td>
                  <xsl:value-of select="ProductLineCode"/>
                </td>
                <td>
                  <xsl:value-of select="LastShipDateString"/>
                </td>
                <td>
                  <xsl:value-of select="LastConsumedDateString"/>
                </td>
                <td align="right" >
                  <xsl:value-of select="format-number(ReorderQty, '#,##0')"/>
                </td>
                <td align="right" >
                  <xsl:value-of select="format-number(RbaFloorUnits, '#,##0')"/>
                </td>
                <td align="right" >
                  <xsl:value-of select="format-number(RequiredUnits, '#,##0')"/>
                </td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td/>
        </tr>
        <tr>
          <td>
            <table class="defaultTable">
              <tr class="head">
                <td align="left">Rack Num</td>
                <td align="left">Batch Bin</td>
                <td align="left">Schedule Num</td>
                <td align="left">Box Num</td>
                <td align="left">Due Date</td>
                <td align="left">Rack Type</td>
                <td align="left">Status</td>
                <td align="left">Required</td>
                <td align="left">Units</td>
                <td align="left">Tran Date</td>
              </tr>
              <xsl:choose>
                <xsl:when test="NextRacks/RbaRack">
                  <xsl:apply-templates select="NextRacks/RbaRack">
                    <xsl:sort select="BatchBin" data-type="number"/>
                  </xsl:apply-templates>
                </xsl:when>
                <xsl:otherwise>
                  <tr>
                    <td colspan="10">None Returned</td>
                  </tr>
                </xsl:otherwise>
              </xsl:choose>
            </table>
            <hr/>
          </td>
        </tr>
      </table>
    </xsl:if>
  </xsl:template>

  <xsl:template match="RbaRack">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="RackNum"/>
      </td>
      <td>
        <xsl:value-of select="BatchBin"/>
      </td>
      <td>
        <xsl:value-of select="ScheduleNum"/>
      </td>
      <td>
        <xsl:value-of select="BoxNum"/>
      </td>
      <td>
        <xsl:value-of select="DueDateString"/>
      </td>
      <td>
        <xsl:value-of select="RackType"/>
      </td>
      <td>
        <xsl:value-of select="HeaderStatus"/>
      </td>
      <td>
        <xsl:if test="IsRequired = 'Y'">
          <xsl:attribute name="class">backGreen</xsl:attribute>
        </xsl:if>
        <xsl:value-of select="IsRequired"/>
      </td>
      <td align="right">
        <xsl:value-of select="Units"/>
      </td>
      <td align="right">
        <xsl:value-of select="TransactionDateString"/>
      </td>
    </tr>
  </xsl:template>


  <xsl:template match="Error">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="Message"/>
      </td>
      <td>
        <xsl:value-of select="Source"/>
      </td>
      <td>
        <xsl:value-of select="StackTrace"/>
      </td>
    </tr>
  </xsl:template>


</xsl:stylesheet>
