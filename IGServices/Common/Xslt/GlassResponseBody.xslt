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

        <div class="title">Spacer Consumption Upload</div>
        <br/>
        <table class="defaultTable">
          <tr class="head">
            <td align="left">Location</td>
            <td align="left">Batch Num</td>
          </tr>
          <tr>
            <td>
              <xsl:value-of select="//PlantCode"/>
            </td>
            <td>
              <xsl:value-of select="//TransferBatchNum"/>
            </td>
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
            <td>
              <xsl:value-of select="//ResponseTimes//ResponseTime//StartTimeString"/>
            </td>
            <td>
              <xsl:value-of select="//ResponseTimes//ResponseTime//ElapsedSeconds"/>
            </td>
            <td>
              <xsl:value-of select="//TransferStatus"/>
            </td>
            <td>
              <xsl:value-of select="//TransferDateString"/>
            </td>
            <td>
              <xsl:value-of select="//HasErrors"/>
            </td>
            <td>
              <xsl:value-of select="//TransferMessage"/>
            </td>
            <td>
            </td>
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
          <tr>
            <td>Assembly Version</td>
            <td>
              <xsl:value-of select="//AssemblyVersion"/>
            </td>
          </tr>
          <tr>
            <td>Write to E1</td>
            <td>
              <xsl:value-of select="//Write"/>
            </td>
          </tr>
          <tr>
            <td>User Name</td>
            <td>
              <xsl:value-of select="//ServiceContext/UserName"/>
            </td>
          </tr>
          <tr>
            <td>Session Id</td>
            <td>
              <xsl:value-of select="//ServiceContext/SessionId"/>
            </td>
          </tr>
          <tr>
            <td>Server Address</td>
            <td>
              <xsl:value-of select="//ServiceContext/ServerAddress"/>
            </td>
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
        <td>
          <xsl:value-of select="Message"/>
        </td>
      </tr>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="ResponseTimes">
    <xsl:for-each select="//ResponseTimes/ResponseTime">
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
          <xsl:value-of select="Description"/>
        </td>
        <td>
          <xsl:value-of select="ElapsedSeconds"/>
        </td>
        <td>
          <xsl:value-of select="ConnectionString"/>
        </td>
      </tr>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="ConnectionStrings">
    <xsl:for-each select="//ConnectionStrings/ConnectionString">
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
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>

