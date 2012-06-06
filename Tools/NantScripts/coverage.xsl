<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
	<!-- created by Yves Lorphelin, largely inspired by the nunitsumary.xsl (see nantcontrib.sourceforge.net)-->
	<xsl:template match="coverage">
		<html>
			<head>
				<title>Epic.NET Code Coverage Report</title>
				<style>BODY {
				font: small verdana, arial, helvetica;
				color:#000000;
				}
				
				P {
				line-height:1.5em;
				margin-top:0.5em; margin-bottom:1.0em;
				}
				H1 {
   				MARGIN: 0px 0px 5px; 
   				FONT: bold larger arial, verdana, helvetica;
	   			
				}
				H2 {
	   			MARGIN-TOP: 1em; MARGIN-BOTTOM: 0.5em; 
	   			FONT: larger verdana,arial,helvetica
				}
				H3 {
		   		MARGIN-BOTTOM: 0.5em; FONT: bold 13px verdana,arial,helvetica
				}
				H4 {
				MARGIN-BOTTOM: 0.5em; FONT: bold 100% verdana,arial,helvetica
				}
				H5 {
   				MARGIN-BOTTOM: 0.5em; FONT: bold 100% verdana,arial,helvetica
				}
				H6 {
	   			MARGIN-BOTTOM: 0.5em; FONT: bold 100% verdana,arial,helvetica
				}	
				.notVisited {background:red; }
				.visited { background: #90ee90; }
                .title { font-size: 12px; font-weight: bold; }
                .assembly { font-size: normal;   font-weight: bold; font-size: 11px}
                .class {font-size:normal; cursor: hand; color: #444444; font-size: 11px}
                .module { color: navy; font-size: 12px; }
                .method {cursor: hand; color: ; font-size: 10px; font-weight: bold; }
                .subtitle { color: black; font-size: 10px; font-weight: bold; }
                .hdrcell  {font-size:9px; background-color: #DDEEFF; }
                .datacell {font-size:9px; background-color: #FFFFEE; text-align: right; }
                .hldatacell {font-size:9px; background-color: #FFCCCC; text-align: right; }
                .detailProcent {  font-size: 9px; font-weight: bold; padding-top: 1px; padding-bottom: 1px; padding-left: 3px; padding-right: 3px;}
				</style>
			</head>
			<body>
				<a name="#top"></a>
				<xsl:call-template name="header" />
				<xsl:call-template name="ModuleSummary" />
				<xsl:call-template name="module" />
				<xsl:call-template name="footer" />
			</body>
		</html>
	</xsl:template>
	<xsl:template name="module">
		<xsl:for-each select="//module[method[seqpnt[@visitcount='0']]]">
			<xsl:sort select="@assembly" />
			<xsl:variable name="module" select="./@assembly" />
			<div class="assembly">
				<a name="#{generate-id($module)}">Module 
					<xsl:value-of select="$module" />
				</a>
			</div>
			<xsl:for-each select="./method[not(./@class = preceding-sibling::method/@class)]">
				<xsl:sort select="@class" />
				<xsl:sort select="@name" />
                <xsl:call-template name="ClassSummary">
                    <xsl:with-param name="module" select="$module" />
                    <xsl:with-param name="class" select="./@class" />
                </xsl:call-template>
			</xsl:for-each>
		</xsl:for-each>
		<xsl:variable name="totalMod" select="count(./method/seqpnt)" />
		<xsl:variable name="notvisitedMod" select="count( ./method/seqpnt[ @visitcount='0'] ) div $totalMod * 100 " />
		<xsl:variable name="visitedMod" select="count(./method/seqpnt[not(@visitcount='0')] ) div $totalMod * 100" />
	</xsl:template>
	<xsl:template name="Methods">
		<xsl:param name="module" />
		<xsl:param name="class" />
		<xsl:for-each select="//method[seqpnt[@visitcount='0'] and (@class = $class) and (parent::module/@assembly=$module)]">
			<xsl:sort select="@name" />
			<xsl:variable name="total" select="count(./seqpnt)" />
			<xsl:variable name="notvisited" select="count(./seqpnt[@visitcount='0'] ) " />
			<xsl:variable name="visited" select="count(./seqpnt[not(@visitcount='0')])" />
			<xsl:variable name="methid" select="generate-id(.)" />
			<table cellpadding="3" cellspacing="0" width="90%">
				<tr>
					<td width="45%" class='method'>
						<xsl:value-of select="@name" />
					</td>
					<td width="55%">
						<xsl:call-template name="detailProcent">
							<xsl:with-param name="visited" select="$visited" />
							<xsl:with-param name="notVisited" select="$notvisited" />
							<xsl:with-param name="total" select="$total" />
						</xsl:call-template>
					</td>
				</tr>
			</table>
			<xsl:call-template name="seqpnt">
				<xsl:with-param name="module" select="$module" />
				<xsl:with-param name="class" select="$class" />
				<xsl:with-param name="id" select="$methid" />
			</xsl:call-template>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="seqpnt">
		<xsl:param name="module" />
		<xsl:param name="class" />
		<xsl:param name="id" />
		<table cellpadding="3" cellspacing="0" border='1' width="90%" bordercolor="black" style="display: block;">
			<xsl:attribute name="id">
				<xsl:value-of select="$id" />
			</xsl:attribute>
			<tr>
				<td class="hdrcell">Visits</td>
				<td class="hdrcell">Line</td>
				<td class="hdrcell">End</td>
				<td class="hdrcell">Column</td>
				<td class="hdrcell">End</td>
				<td class="hdrcell">Document</td>
			</tr>
			<xsl:for-each select="./seqpnt">
				<xsl:sort select="@line" />
				<tr>
					<td class="datacell">
						<xsl:attribute name="class">
							<xsl:choose>
								<xsl:when test="@visitcount = 0">hldatacell</xsl:when>
								<xsl:otherwise>datacell</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						<xsl:value-of select="@visitcount" />
					</td>
					<td class="datacell">
						<xsl:value-of select="@line" />
					</td>
					<td class="datacell">
						<xsl:value-of select="@endline" />
					</td>
					<td class="datacell">
						<xsl:value-of select="@column" />
					</td>
					<td class="datacell">
						<xsl:value-of select="@endcolumn" />
					</td>
					<td class="datacell">
						<xsl:value-of select="@document" />
					</td>
				</tr>
			</xsl:for-each>
		</table>
	</xsl:template>
	<!-- Class Summary -->
	<xsl:template name="ClassSummary">
		<xsl:param name="module" />
		<xsl:param name="class" />
		<xsl:variable name="total" select="count(//seqpnt[(parent::method/parent::module/@assembly=$module) and (parent::method/@class=$class) ])" />
		<xsl:variable name="notvisited" select="count(//seqpnt[(parent::method/parent::module/@assembly=$module)and (parent::method/@class=$class) and (@visitcount='0')] )" />
		<xsl:variable name="visited" select="count(//seqpnt[(parent::method/parent::module/@assembly=$module) and (parent::method/@class=$class) and (not(@visitcount='0'))] )" />
		<xsl:variable name="newid" select="concat (generate-id(), 'class')" />
		<xsl:if test="not($notvisited=0)">
		<table width='90%'>
			<tr>
				<td width="40%" class="class">
					<xsl:value-of select="$class" />
				</td>
				<td width="60%">
					<xsl:call-template name="detailProcent">
						<xsl:with-param name="visited" select="$visited" />
						<xsl:with-param name="notVisited" select="$notvisited" />
						<xsl:with-param name="total" select="$total" />
					</xsl:call-template>
				</td>
			</tr>
			<tr>
				<table style="display: block;" width="100%">
					<tr>
						<td>
							<xsl:attribute name="id">
								<xsl:value-of select="$newid" />
							</xsl:attribute>
							<xsl:call-template name="Methods">
								<xsl:with-param name="module" select="$module" />
								<xsl:with-param name="class" select="$class" />
							</xsl:call-template>
						</td>
					</tr>
				</table>
			</tr>
		</table>
		<hr size="1" width='90%' align='left' style=" border-bottom: 1px dotted #999;" />
		</xsl:if>
	</xsl:template>
	<xsl:template name="ClassSummaryDetail">
		<xsl:param name="module" />
		<xsl:variable name="total" select="count(./method/seqpnt)" />
		<xsl:variable name="notVisited" select="count( ./method/seqpnt[ @visitcount='0'] )" />
		<xsl:variable name="visited" select="count(./method/seqpnt[not(@visitcount='0')] )" />
		<td width="35%">
			<div class="assembly">
				<a href="#{generate-id($module)}">
					<xsl:value-of select="$module" />
				</a>
			</div>
		</td>
		<td width="65%">
			<xsl:call-template name="detailProcent">
				<xsl:with-param name="visited" select="$visited" />
				<xsl:with-param name="notVisited" select="$notVisited" />
				<xsl:with-param name="total" select="$total" />
			</xsl:call-template>
		</td>
	</xsl:template>
	<!-- Modules Summary -->
	<xsl:template name="ModuleSummary">
		<H2>Modules summary</H2>
		<xsl:for-each select="//module[method[seqpnt[@visitcount='0']]]">
			<xsl:sort select="@assembly" />
			<table width='90%'>
				<tr>
					<xsl:call-template name="ModuleSummaryDetail">
						<xsl:with-param name="module" select="./@assembly" />
					</xsl:call-template>
				</tr>
			</table>
		</xsl:for-each>
		<hr size="1" />
	</xsl:template>
	<xsl:template name="ModuleSummaryDetail">
		<xsl:param name="module" />
		<xsl:variable name="total" select="count(./method/seqpnt)" />
		<xsl:variable name="notVisited" select="count( ./method/seqpnt[ @visitcount='0'] )" />
		<xsl:variable name="visited" select="count(./method/seqpnt[not(@visitcount='0')] )" />
		<td width="30%">
			<div class="assembly">
				<a href="#{generate-id($module)}">
					<xsl:value-of select="$module" />
				</a>
			</div>
		</td>
		<td width="70%">
			<xsl:call-template name="detailProcent">
				<xsl:with-param name="visited" select="$visited" />
				<xsl:with-param name="notVisited" select="$notVisited" />
				<xsl:with-param name="total" select="$total" />
			</xsl:call-template>
		</td>
	</xsl:template>
	<!-- General Header -->
	<xsl:template name="header">
		<h1>
			<b>Epic.NET</b> Code Coverage Report
		</h1>
		<hr size="1" />
	</xsl:template>
	<xsl:template name="footer">
		<hr size="1" />
		<a class="detailProcent" href="#{top}">Top</a>
	</xsl:template>
	<!-- draw % table-->
	<xsl:template name="detailProcent">
		<xsl:param name="visited" />
		<xsl:param name="notVisited" />
		<xsl:param name="total" />
		<table width="100%" class="detailProcent">
			<tr>
				<xsl:if test="not($notVisited=0)">
					<td class="notVisited">
						<xsl:attribute name="width">
							<xsl:value-of select="concat($notVisited div $total * 100,'%')" />
						</xsl:attribute>
						<xsl:value-of select="concat (format-number($notVisited div $total * 100,'#.##'),'%')" />
					</td>
				</xsl:if>
				<xsl:if test="not ($visited=0)">
					<td class="visited">
						<xsl:attribute name="width">
							<xsl:value-of select="concat($visited div $total * 100,'%')" />
						</xsl:attribute>
						<xsl:value-of select="concat (format-number($visited div $total * 100,'#.##'), '%')" />
					</td>
				</xsl:if>
			</tr>
		</table>
	</xsl:template>
</xsl:stylesheet>