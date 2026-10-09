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
          .defaultTable td, table
          {
          border: 1px solid black;
          border-collapse:collapse;
          padding:2px;
          }
        </style>
      </head>
      <body>
        <div class="title">Glass Inventory Transaction Upload</div>
        <br/>
        <xsl:if test="ArrayOfGlassInventoryTransaction">
          <div class="title">Glass Inventory Transactions</div>
          <br/>
          <table class="defaultTable">
            <tr class="head">
              <td align="left">
                TransactionId
              </td>
              <td align="left">
                WOrderNum
              </td>
              <td align="left">
                ScheduleNum
              </td>
              <td align="left">
                UnitId
              </td>
              <td align="left">
                LiteNum
              </td>
              <td align="left">
                LiteName
              </td>
              <td align="right">
                Quantity
              </td>
            </tr>
            <xsl:apply-templates select="ArrayOfGlassInventoryTransaction/GlassInventoryTransaction">
              <xsl:sort select="GLStockID"/>
            </xsl:apply-templates>
            <tr>
              <td colspan="3"/>
              <td align="right" class="numeric">
                <xsl:value-of select="format-number(sum(ArrayOfGlassInventoryTransaction/GlassInventoryTransaction/Quantity), '#,###')"/>
              </td>
            </tr>
          </table>
          <br/>
        </xsl:if>
        <br/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="GlassInventoryTransaction">
    <tr>
      <xsl:attribute name="class">
        <xsl:choose>
          <xsl:when test="position() mod 2 = 0">even</xsl:when>
          <xsl:otherwise>odd</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <td>
        <xsl:value-of select="TransactionId"/>
      </td>
      <td>
        <xsl:value-of select="WOrderNum"/>
      </td>
      <td>
        <xsl:value-of select="ScheduleNum"/>
      </td>
      <td>
        <xsl:value-of select="UnitId"/>
      </td>
      <td>
        <xsl:value-of select="LiteNum"/>
      </td>
      <td>
        <xsl:value-of select="LiteName"/>
      </td>
      <td align="right" >
        <xsl:value-of select="Quantity"/>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>
