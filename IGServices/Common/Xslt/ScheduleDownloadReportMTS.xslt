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
          td.numeric { text-align:right; width:80px;  }
          td.noBorder {border: none; }
          .error {color: red;}
          .title {font-weight:bold; text-decoration: underline;}
          .depth1 {text-indent: 0px;}
          .depth2 {text-indent: 5px;}
          .depth3 {text-indent: 10px;}
          .depth4 {text-indent: 15px;}
          .depth5 {text-indent: 20px;}
          .depth6 {text-indent: 25px;}
          .depth7 {text-indent: 30px;}
          .depth8 {text-indent: 35px;}
          .depth9 {text-indent: 40px;}
          .footerTotals {font-weight:bold;color: white;background: #565656; }
          tr.odd { background: #ffffff; }
          tr.even { background: #D5CFCF; }
          .defaultTable td, table
          {
          border: 1px solid black;
          border-collapse:collapse;
          padding:2px;
          }
        </style>
      </head>
      <body>
        <div class="title">MTS Schedule Download</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">
              Location
            </td>
            <td align="left">
              E1 Schedule Num
            </td>
            <td align="left">
              E1 Schedule Date
            </td>
            <td align="left">
              E1 Batch Num
            </td>
            <td align="right">
              Start Date
            </td>
            <td align="right">
              End Date
            </td>
            <td align="right">
              Elapsed Seconds
            </td>
          </tr>
          <xsl:apply-templates select="ScheduleHeader/ScheduleHeader">
          </xsl:apply-templates>
        </table>
        <br/>
        <xsl:if test="//Error">
          <div class="title">Errors</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">
                Message
              </td>
              <td align="left">
                Source
              </td>
              <td align="left">
                Stack Trace
              </td>
            </tr>
            <xsl:apply-templates select="//Error">
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>
        <xsl:if test="CustomerSummary">
          <div class="title">Batch Summary</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td colspan="6">
              </td>
              <td colspan="2" align="center">
                Orders
              </td>
              <td colspan="7" align="center">
                Units
              </td>
            </tr>
            <tr class="head">
              <td align="left">
                Work Order Num
              </td>
              <td align="left">
                Cust Num
              </td>
              <td align="left">
                Cust Name
              </td>
              <td align="left">
                Min Date
              </td>
              <td align="left">
                Max Date
              </td>
              <td align="right">
                Sales Order
              </td>
              <td align="right">
                Work Order
              </td>
              <td align="right">
                Mono Pick
              </td>
              <td align="right">
                Mono Cut
              </td>
              <td align="right">
                IG2 Pick
              </td>
              <td align="right">
                IG2 Line
              </td>
              <td align="right">
                IG3 Pick
              </td>
              <td align="right">
                IG3 Line
              </td>
              <td align="right">
                Total
              </td>
            </tr>
            <xsl:apply-templates select="CustomerSummary/CustomerLines/CustomerLine">
              <xsl:sort select="WorkOrderNum"/>
              <xsl:sort select="Customer"/>
            </xsl:apply-templates>
            <tr class="footerTotals">
              <td colspan="5"/>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(CustomerSummary/CustomerLines/CustomerLine/SalesOrders), '#,##0')"/>
              </td>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(CustomerSummary/CustomerLines/CustomerLine/WorkOrders), '#,##0')"/>
              </td>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(CustomerSummary/CustomerLines/CustomerLine/MonoPickUnits), '#,##0')"/>
              </td>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(CustomerSummary/CustomerLines/CustomerLine/MonoCutUnits), '#,##0')"/>
              </td>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(CustomerSummary/CustomerLines/CustomerLine/IG2PickUnits), '#,##0')"/>
              </td>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(CustomerSummary/CustomerLines/CustomerLine/IG2LineUnits), '#,##0')"/>
              </td>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(CustomerSummary/CustomerLines/CustomerLine/IG3PickUnits), '#,##0')"/>
              </td>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(CustomerSummary/CustomerLines/CustomerLine/IG3LineUnits), '#,##0')"/>
              </td>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(CustomerSummary/CustomerLines/CustomerLine/TotalUnits), '#,##0')"/>
              </td>
            </tr>
          </table>
          <br/>
        </xsl:if>
        <xsl:if test="ProcessResponse/ProcessSteps">
          <div class="title">Process Steps</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">
                Id
              </td>
              <td align="left">
                Name
              </td>
              <td align="left">
                Start Date
              </td>
              <td align="left">
                End Date
              </td>
              <td align="left">
                Elapsed(Sec)
              </td>
              <td align="left">
                Rows Affected
              </td>
              <td align="left">
                Has Errors
              </td>
              <td align="left">
                Messages
              </td>
            </tr>
            <xsl:apply-templates select="ProcessResponse/ProcessSteps/ProcessStep">
              <xsl:sort select="StartDate"/>
            </xsl:apply-templates>
            <tr>
              <td colspan="4"/>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(ProcessResponse/ProcessSteps/ProcessStep/ElapsedSeconds), '#,##0.00')"/>
              </td>
              <td colspan="2"/>
            </tr>
          </table>
          <br/>
        </xsl:if>
        <xsl:if test="ProcessResponse/ConnectionStrings">
          <div class="title">Connection Strings</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">
                Name
              </td>
              <td align="left">
                Value
              </td>
            </tr>
            <xsl:apply-templates select="ProcessResponse/ConnectionStrings/ConnectionString">
              <xsl:sort select="Name"/>
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>
        <br/>
      </body>
    </html>
  </xsl:template>


  <xsl:template match="ScheduleHeader">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="Plant"/>
      </td>
      <td>
        <xsl:value-of select="ScheduleNum"/>
      </td>
      <td>
        <xsl:value-of select="ScheduleDate"/>
      </td>
      <td>
        <xsl:value-of select="BatchNum"/>
      </td>
      <td>
        <xsl:value-of select="StartDate"/>
      </td>
      <td>
        <xsl:value-of select="EndDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="CustomerLine">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="WorkOrderNum"/>
      </td>
      <td>
        <xsl:value-of select="Customer"/>
      </td>
      <td>
        <xsl:value-of select="CustomerName"/>
      </td>
      <td>
        <xsl:value-of select="MinRequestedDate"/>
      </td>
      <td>
        <xsl:value-of select="MaxRequestedDate"/>
      </td>
      <td align="right">
        <xsl:value-of select="SalesOrders"/>
      </td>
      <td align="right">
        <xsl:value-of select="WorkOrders"/>
      </td>
      <td align="right">
        <xsl:value-of select="MonoPickUnits"/>
      </td>
      <td align="right">
        <xsl:value-of select="MonoCutUnits"/>
      </td>
      <td align="right">
        <xsl:value-of select="IG2PickUnits"/>
      </td>
      <td align="right">
        <xsl:value-of select="IG2LineUnits"/>
      </td>
      <td align="right">
        <xsl:value-of select="IG3PickUnits"/>
      </td>
      <td align="right">
        <xsl:value-of select="IG3LineUnits"/>
      </td>
      <td align="right">
        <xsl:value-of select="TotalUnits"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="ProcessResponse/ProcessSteps/ProcessStep">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="Id"/>
      </td>
      <td>
        <xsl:value-of select="Name"/>
      </td>
      <td>
        <xsl:value-of select="StartDateChar"/>
      </td>
      <td>
        <xsl:value-of select="EndDateChar"/>
      </td>
      <td align="right">
        <xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
      </td>
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="HasErrors"/>
      </td>
      <td>
        <xsl:value-of select="Errors/Error/Message"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="ProcessResponse/ConnectionStrings/ConnectionString">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="Name"/>
      </td>
      <td>
        <xsl:value-of select="Value"/>
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
