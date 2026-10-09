<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:ms="urn:schemas-microsoft-com:xslt">

  <xsl:template match="/">
    <html>
      <head>
        <style>
          body,td,p,th  { font-family: arial; font-size: 8pt; }
          tr.head { border : 0px solid black; padding: 0 0px; background: #ddddff; font-weight:bold;}
          td.numeric { text-align:right; width:80px;  }
          .error {color: red;}
          .title {font-weight:bold; text-decoration: underline;}
          tr.odd { background: #ffffff; }
          tr.even { background: #D5CFCF; }
          .defaultTable td
          {
          border: 0px solid black;
          border-spacing: 0;
          border-collapse:collapse;
          padding:2px;
          }
        </style>
      </head>
      <body>

        <div class="title">Boxing Schedule</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Location</td>
            <td align="left">Schedule Num</td>
            <td align="left">Batch Num</td>
            <td align="left">Units</td>
            <td align="left">Work Orders</td>
            <td align="left">Boxes</td>
          </tr>
          <tr>
            <td><xsl:value-of select="//PlantCode"/> - <xsl:value-of select="//PlantNum"/></td>
            <td><xsl:value-of select="//ScheduleNum"/></td>
            <td><xsl:value-of select="//TransferBatchNum"/></td>
            <td><xsl:value-of select="//BoxingSummary/Units"/></td>
            <td><xsl:value-of select="//BoxingSummary/WorkOrders"/></td>
            <td><xsl:value-of select="//BoxingSummary/Boxes"/></td>
          </tr>
        </table>

        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Run Date</td>
            <td align="left">Elapsed Seconds</td>
            <td align="left">Transfer Status</td>
            <td align="left">Transfer Date</td>
            <td align="left">Has Errors</td>
            <td align="left">Message</td>
          </tr>
          <tr>
            <td><xsl:value-of select="//ResponseTimes//ResponseTime//StartTimeString"/></td>
            <td><xsl:value-of select="//ResponseTimes//ResponseTime//ElapsedSeconds"/></td>
            <td><xsl:value-of select="//BoxingSchedule//TransferStatus"/></td>
            <td><xsl:value-of select="//BoxingSchedule//TransferDateString"/></td>
            <td><xsl:value-of select="//HasErrors"/></td>
            <td><xsl:value-of select="//Message"/></td>
          </tr>
        </table>

        <xsl:choose>
          <xsl:when test="count(//Errors/Error) > 0">
            <br/>
            <table class="defaultTable">
              <tr class="head">
                <td align="left">Error Message</td>
              </tr>
              <xsl:apply-templates select="//Errors"/>
            </table>
          </xsl:when>
        </xsl:choose>

        <br/>
        <div class="title">Boxing Customer Lines</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Box Num</td>
            <td align="left">Box Type</td>
            <td align="left">Rack Type Desc</td>
            <td align="left">Customer</td>
            <td align="left">Min Unit ID</td>
            <td align="left">Max Unit ID</td>
            <td align="left">Due Date</td>
            <td align="right">Units Qty</td>
            <td align="left">Min WO Num</td>
            <td align="left">Max WO Num</td>
          </tr>
          <xsl:apply-templates select="//BoxingSummary/BoxCustomerLines"/>
          <tr>
            <td colspan="7"></td>
            <td align="right" class="numeric">
              <xsl:value-of select="sum(//BoxingSummary/BoxCustomerLines/BoxCustomerLine/UnitsQty)"/>
            </td>
          </tr>
        </table>

        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Name</td>
            <td align="left">Description</td>
            <td align="left">Elapsed Seconds</td>
            <td align="left">Connection String</td>
          </tr>
          <xsl:apply-templates select="//ResponseTimes"/>
        </table>

        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">End Point</td>
            <td align="left">Connection String</td>
          </tr>
          <xsl:apply-templates select="//ConnectionStrings"/>
        </table>

        <br/>
        <table class="defaultTable">
          <tr><td>Assembly Version</td>
            <td><xsl:value-of select="//AssemblyVersion"/></td>
          </tr>
          <tr>
            <td>Write to E1</td>
            <td><xsl:value-of select="//Write"/></td>
          </tr>
          <tr>
            <td>User Name</td>
            <td><xsl:value-of select="//ServiceContext/UserName"/></td>
          </tr>
          <tr>
            <td>Session Id</td>
            <td><xsl:value-of select="//ServiceContext/SessionId"/></td>
          </tr>
          <tr>
            <td>Server Address</td>
            <td><xsl:value-of select="//ServiceContext/ServerAddress"/></td>
          </tr>
        </table>

      </body>
    </html>
  </xsl:template>

  <xsl:template match="Errors">
    <xsl:for-each select="//Errors/Error">
      <tr>
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">even</xsl:when>
            <xsl:otherwise>odd</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <td><xsl:value-of select="Message"/></td>
      </tr>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="BoxLines">
    <xsl:for-each select="//BoxingSummary/BoxLines/BoxLine">
      <tr>
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">even</xsl:when>
            <xsl:otherwise>odd</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <td><xsl:value-of select="BoxNum"/></td>
        <td><xsl:value-of select="BoxType"/></td>
        <td><xsl:value-of select="RackTypeDesc"/></td>
        <td align="right"><xsl:value-of select="UnitsQty"/></td>
      </tr>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="BoxWorkOrderLines">
    <xsl:for-each select="//BoxingSummary/BoxWorkOrderLines/BoxWorkOrderLine">
      <tr>
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">even</xsl:when>
            <xsl:otherwise>odd</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <td><xsl:value-of select="BoxNum"/></td>
        <td><xsl:value-of select="BoxType"/></td>
        <td><xsl:value-of select="RackTypeDesc"/></td>
        <td><xsl:value-of select="WorkOrderNum"/></td>
        <td align="right"><xsl:value-of select="UnitsQty"/></td>
      </tr>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="BoxCustomerLines">
    <xsl:for-each select="//BoxingSummary/BoxCustomerLines/BoxCustomerLine">
      <tr>
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">even</xsl:when>
            <xsl:otherwise>odd</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <td><xsl:value-of select="BoxNum"/></td>
        <td><xsl:value-of select="BoxType"/></td>
        <td><xsl:value-of select="RackTypeDesc"/></td>
        <td><xsl:value-of select="CustomerName"/></td>
        <td><xsl:value-of select="MinUnitID"/></td>
        <td><xsl:value-of select="MaxUnitID"/></td>
        <td><xsl:value-of select="ms:format-date(//MinDueDate, 'MM/dd/yyyy','en-US')"/>&#160;</td>
        <td align="right"><xsl:value-of select="UnitsQty"/></td>
        <td><xsl:value-of select="MinWorkOrderNum"/></td>
        <td><xsl:value-of select="MaxWorkOrderNum"/></td>
      </tr>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="ResponseTimes">
    <xsl:for-each select="ResponseTime">
      <tr>
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">even</xsl:when>
            <xsl:otherwise>odd</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <td><xsl:value-of select="Name"/></td>
        <td><xsl:value-of select="Description"/></td>
        <td><xsl:value-of select="ElapsedSeconds"/></td>
        <td><xsl:value-of select="ConnectionString"/></td>
      </tr>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="ConnectionStrings">
    <xsl:for-each select="ConnectionString">
      <tr>
        <xsl:attribute name="class">
          <xsl:choose>
            <xsl:when test="position() mod 2 = 0">even</xsl:when>
            <xsl:otherwise>odd</xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <td><xsl:value-of select="Name"/></td>
        <td><xsl:value-of select="Value"/></td>
      </tr>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>

