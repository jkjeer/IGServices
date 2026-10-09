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
          tr.odd { background: #ffffff; }
          tr.even { background: #D5CFCF; }
          tr.stepodd { background: #ffffcc; }
          tr.stepeven { background: #e6ffe6; }
          .defaultTable td, table
          {
          border: 1px solid black;
          border-collapse:collapse;
          padding:2px;
          }
        </style>
      </head>
      <body>
        <div class="title">Spacer Consumption Upload</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Location</td>
            <td align="left">Batch Num</td>
            <td align="left">Spacers</td>
            <td align="left">Items</td>
            <td align="left">Transfer Status</td>
            <td align="left">Has Errors</td>
            <td align="left">Start Date</td>
            <td align="right">Elapsed Seconds</td>
          </tr>
          <xsl:apply-templates select="SummaryNode">
          </xsl:apply-templates>
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

        <xsl:if test="ProcessResponse/Messages/Message">
          <div class="title">Messages</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Name</td>
              <td align="left">Value</td>
            </tr>
            <xsl:apply-templates select="ProcessResponse/Messages/Message"/>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="SpacerConsumptions/ArrayOfSpacerConsumption/SpacerConsumption">
          <div class="title">Summary</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">ItemNum</td>
              <td align="left">SourceRows</td>
              <td align="left">Quantity</td>
              <td align="left">ProductionDate</td>
              <td align="left">MinWrittenDate</td>
              <td align="left">MaxWrittenDate</td>
            </tr>
            <xsl:apply-templates select="SpacerConsumptions"/>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="ProcessResponse">
          <div class="title">Process Steps</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Name</td>
              <td align="left">Start Date</td>
              <td align="left">End Date</td>
              <td align="left">Elapsed(Sec)</td>
              <td align="left">Rows Affected</td>
              <td align="left">Messages</td>
            </tr>
            <xsl:apply-templates select="ProcessResponse">
              <xsl:sort select="StartDate"/>
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>

        <xsl:if test="ProcessResponse/ConnectionStrings">
          <div class="title">Connection Strings</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">Name</td>
              <td align="left">Value</td>
            </tr>
            <xsl:apply-templates select="ProcessResponse/ConnectionStrings/ConnectionString">
              <xsl:sort select="Name"/>
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>

        <br/>
        <table class="defaultTable">
          <tr>
            <td>Assembly Version</td>
            <td>
              <xsl:value-of select="ProcessResponse/AssemblyVersion"/>
            </td>
          </tr>
          <tr>
            <td>User Name</td>
            <td>
              <xsl:value-of select="ProcessResponse/ServiceContext/UserName"/>
            </td>
          </tr>
          <tr>
            <td>Session Id</td>
            <td>
              <xsl:value-of select="ProcessResponse/ServiceContext/SessionId"/>
            </td>
          </tr>
          <tr>
            <td>Server Address</td>
            <td>
              <xsl:value-of select="ProcessResponse/ServiceContext/ServerAddress"/>
            </td>
          </tr>
        </table>

      </body>
    </html>
  </xsl:template>

  <xsl:template match="SummaryNode/Header">
    <tr>
      <td>
        <xsl:value-of select="Location"/>
      </td>
      <td>
        <xsl:value-of select="TransferBatchNum"/>
      </td>
      <td>
        <xsl:value-of select="Spacers"/>
      </td>
      <td>
        <xsl:value-of select="Items"/>
      </td>
      <td>
        <xsl:value-of select="TransferStatus"/>
      </td>
      <td>
        <xsl:value-of select="HasErrors"/>
      </td>
      <td>
        <xsl:value-of select="StartDateString"/>
      </td>
      <td align="right">
        <xsl:value-of select="format-number(ElapsedSeconds, '#,##0.00')"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="SpacerConsumptions/ArrayOfSpacerConsumption/SpacerConsumption">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="ItemNum"/>
      </td>
      <td>
        <xsl:value-of select="SourceRows"/>
      </td>
      <td>
        <xsl:value-of select="Quantity"/>
      </td>
      <td>
        <xsl:value-of select="ProductionDate"/>
      </td>
      <td>
        <xsl:value-of select="MinWrittenDate"/>
      </td>
      <td>
        <xsl:value-of select="MaxWrittenDate"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="ProcessResponse">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td class="depth{count(ancestor::*)}">
        <xsl:value-of select="ProcessName"/>
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
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="Errors/Error/Message"/>
      </td>
    </tr>
    <xsl:if test="ProcessSteps/ProcessStep">
      <xsl:apply-templates select="ProcessSteps/ProcessStep">
        <xsl:sort select="StartDate"/>
      </xsl:apply-templates>
    </xsl:if>
  </xsl:template>

  <xsl:template match="ProcessStep">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">stepeven</xsl:when>
          <xsl:otherwise>stepeven</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td class="depth{count(ancestor::*)}">
        <xsl:value-of select="Name"/>
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
      <td align="right">
        <xsl:value-of select="RowsAffected"/>
      </td>
      <td>
        <xsl:value-of select="Errors/Error/Message"/>
      </td>
    </tr>
    <xsl:if test="ProcessResponses">
      <xsl:apply-templates select="ProcessResponses/ProcessResponse">
        <xsl:sort select="StartDate"/>
      </xsl:apply-templates>
    </xsl:if>
  </xsl:template>

  <xsl:template match="ConnectionString">
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

  <xsl:template match="//Error">
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

  <xsl:template match="Message">
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

</xsl:stylesheet>
