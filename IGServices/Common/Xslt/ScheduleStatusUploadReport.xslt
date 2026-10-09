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
        <!-- Header -->
        <div class="title">E1 Schedule Status Update</div>
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
        <!-- Error -->
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
        <!-- Statements -->
				<xsl:if test="//SqlDmlResponse">
					<br/>
					<div class="title">Statements</div>
					<xsl:apply-templates select="//SqlDmlResponse">
					</xsl:apply-templates>
				</xsl:if>
        <!-- Messages -->
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
        <!-- Process Steps -->
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
        <!-- Connection Strings -->
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

  <!-- Templates -->
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
  <xsl:template match="SqlDmlResponse">
		<ul>
			<li>
				<xsl:value-of select="ConnectionString"/>
			</li>
			<li>
				<xsl:value-of select="Statement"/>
			</li>
			<ul>
				<xsl:apply-templates select="Parameters/Parameter">
				</xsl:apply-templates>
			</ul>
		</ul>
	</xsl:template>
  <xsl:template match="Parameter">
		<li>
			<xsl:value-of select="Name"/> = <xsl:value-of select="Value"/>
		</li>
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
