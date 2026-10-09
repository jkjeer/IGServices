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
          tr.odd { background: #ffffff; }
          tr.even { background: #D5CFCF; }
          .footerTotals {font-weight:bold;color: white;background: #565656; }
          .defaultTable td, table
          {
          border: 1px solid black;
          border-collapse:collapse;
          padding:2px;
          }
        </style>
      </head>
      <body>
        <div class="title">Glass Business Transaction Upload</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">
              Location
            </td>
            <td align="left">
              Batch Num
            </td>
            <td align="left">
              Transactions
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
          <xsl:apply-templates select="Header/Header">
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
        <br/>
        <xsl:if test="Messages/ArrayOfMessage/Message">
          <div class="title">Messages</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">
                Name
              </td>
              <td align="left">
                Message
              </td>
              <td align="left">
                Severity
              </td>
            </tr>
            <xsl:apply-templates select="Messages/ArrayOfMessage/Message">
            </xsl:apply-templates>
          </table>
          <br/>
        </xsl:if>
        <br/>
        <xsl:if test="GlassBusinessTransactions">
          <div class="title">Transactions</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td colspan="10">Shop Floor System</td>
              <td colspan="3">Business System</td>
            </tr>
            <tr class="head">
              <td align="left">
                Transaction Source
              </td>
              <td align="left">
                Transaction Id
              </td>
              <td align="left">
                Transaction Date
              </td>
              <td align="left">
                Schedule Num
              </td>
              <td align="left">
                Lot Num
              </td>
              <td align="left">
                Serial Num
              </td>
              <td align="left">
                Lite Num
              </td>
              <td align="left">
                Production Line
              </td>
              <td align="left">
                Line Operation
              </td>
              <td align="left">
                Comments
              </td>
              <td align="left">
                Transfer Id
              </td>
              <td align="left">
                Transaction Type
              </td>
              <td align="right">
                Quantity
              </td>
            </tr>
            <xsl:apply-templates select="GlassBusinessTransactions/ArrayOfGlassBusinessTransaction/GlassBusinessTransaction">
              <xsl:sort select="TransferId"/>
            </xsl:apply-templates>
            <tr class="footerTotals">
              <td colspan="12"/>
              <td align="right"
                  class="numeric">
                <xsl:value-of select="format-number(sum(GlassBusinessTransactions/ArrayOfGlassBusinessTransaction/GlassBusinessTransaction/Quantity), '#,###')"/>
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
              <td align="right"
                  class="numeric">
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
        <br/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="Header">
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
        <xsl:value-of select="BatchNum"/>
      </td>
      <td>
        <xsl:value-of select="Transactions"/>
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
  <xsl:template match="GlassBusinessTransaction">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="TransactionSourceCode"/>
      </td>
      <td>
        <xsl:value-of select="TransactionId"/>
      </td>
      <td>
        <xsl:value-of select="TransactionDateChar"/>
      </td>
      <td>
        <xsl:value-of select="ScheduleNum"/>
      </td>
      <td>
        <xsl:value-of select="LotNum"/>
      </td>
      <td>
        <xsl:value-of select="SerialNum"/>
      </td>
      <td>
        <xsl:value-of select="LiteNum"/>
      </td>
      <td>
        <xsl:value-of select="ProductionLineCode"/>
      </td>
      <td>
        <xsl:value-of select="LineOperationCode"/>
      </td>
      <td>
        <xsl:value-of select="Comments"/>
      </td>
      <td>
        <xsl:value-of select="TransferId"/>
      </td>
      <td align="center">
        <xsl:value-of select="TransactionType"/>
      </td>
      <td align="right" >
        <xsl:value-of select="Quantity"/>
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
      <td>
        <xsl:value-of select="Severity"/>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
