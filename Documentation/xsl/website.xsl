<?xml version="1.0" encoding="ASCII"?>
<!--This file was created automatically by html2xhtml-->
<!--from the HTML stylesheets.-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exsl="http://exslt.org/common" xmlns="http://www.w3.org/1999/xhtml" version="1.0" exclude-result-prefixes="exsl">

<xsl:import href="http://docbook.sourceforge.net/release/xsl/current/xhtml/docbook.xsl"/>

<xsl:import href="http://docbook.sourceforge.net/release/xsl/current/xhtml/chunk-common.xsl"/>

<xsl:template name="user.head.content">
  <meta charset='utf-8'/>

  <title>Epic - dominant domains</title>
  <link rel="icon" href="favicon.ico" />
  <link rel="alternate" type="application/atom+xml" title="Development log"  href="atom.xml" />
  <xsl:comment>[if !IE 7]>
	<style type="text/css">
		#wrap {display:table;height:100%}
	</style>
  &lt;![endif]</xsl:comment>
  <xsl:comment>[if lt IE 8]>
	<style type="text/css">
		#header {padding-top: 9px}
	</style>
  &lt;![endif]</xsl:comment>
  <xsl:comment>[if IE 8]>
	<style type="text/css">
		#wrap {width:100%}
	</style>
  &lt;![endif]</xsl:comment>
</xsl:template>

<xsl:template name="make.toc">
  <xsl:param name="toc-context" select="."/>
  <xsl:param name="toc.title.p" select="true()"/>
  <xsl:param name="nodes" select="/NOT-AN-ELEMENT"/>

  <xsl:variable name="nodes.plus" select="$nodes | qandaset"/>

  <xsl:choose>
    <xsl:when test="$manual.toc != ''">
      <xsl:variable name="id">
        <xsl:call-template name="object.id"/>
      </xsl:variable>
      <xsl:variable name="toc" select="document($manual.toc, .)"/>
      <xsl:variable name="tocentry" select="$toc//tocentry[@linkend=$id]"/>
      <xsl:if test="$tocentry and $tocentry/*">
        <div class="toc">
          <xsl:element name="{$toc.list.type}">
            <xsl:call-template name="manual-toc">
              <xsl:with-param name="tocentry" select="$tocentry/*[1]"/>
            </xsl:call-template>
          </xsl:element>
        </div>
      </xsl:if>
    </xsl:when>
    <xsl:otherwise>
      <xsl:choose>
        <xsl:when test="$qanda.in.toc != 0">
          <xsl:if test="$nodes.plus">
            <div class="toc">
              <xsl:element name="{$toc.list.type}">
                <xsl:apply-templates select="$nodes.plus" mode="toc">
                  <xsl:with-param name="toc-context" select="$toc-context"/>
                </xsl:apply-templates>
              </xsl:element>
            </div>
          </xsl:if>
        </xsl:when>
        <xsl:otherwise>
          <xsl:if test="$nodes">
            <div class="toc">
              <xsl:element name="{$toc.list.type}">
                <xsl:element name="{$toc.listitem.type}">
                  <a href="/doc/" class="toc-title">
                  <xsl:call-template name="gentext">
                    <xsl:with-param name="key">TableofContents</xsl:with-param>
                  </xsl:call-template>
                  </a>
                </xsl:element>
                <xsl:apply-templates select="$nodes" mode="toc">
                  <xsl:with-param name="toc-context" select="$toc-context"/>
                </xsl:apply-templates>
              </xsl:element>
            </div>
          </xsl:if>
        </xsl:otherwise>
      </xsl:choose>

    </xsl:otherwise>
  </xsl:choose>
</xsl:template>

<xsl:template name="chunk-element-content"> 
  <xsl:param name="prev"/> 
  <xsl:param name="next"/> 
  <xsl:param name="nav.context"/> 
  <xsl:param name="content"> 
    <xsl:apply-imports/> 
  </xsl:param> 
 
  <xsl:call-template name="user.preroot"/> 
 
  <html> 
    <xsl:call-template name="html.head"> 
      <xsl:with-param name="prev" select="$prev"/> 
      <xsl:with-param name="next" select="$next"/> 
    </xsl:call-template> 
 
    <body> 
      <xsl:call-template name="body.attributes"/> 

  <div id="wrap">
    <div id="header">
      <a href="http://github.com/Shamar/Epic.NET/"><img style="position: absolute; top: 0; left: 0; border: 0;" src="http://s3.amazonaws.com/github/ribbons/forkme_left_orange_ff7600.png" alt="Fork me on GitHub" /></a>
      <div class="title">Epic</div>
      <span>dominant domains</span>
    </div>
    <div id="main">
      <div id="content">
      <xsl:call-template name="user.header.navigation"/> 
 
      <xsl:call-template name="header.navigation"> 
        <xsl:with-param name="prev" select="$prev"/> 
        <xsl:with-param name="next" select="$next"/> 
        <xsl:with-param name="nav.context" select="$nav.context"/> 
      </xsl:call-template> 
 
      <xsl:call-template name="user.header.content"/> 
 
      <xsl:copy-of select="$content"/> 
 
      <xsl:call-template name="user.footer.content"/> 
 
      <xsl:call-template name="footer.navigation"> 
        <xsl:with-param name="prev" select="$prev"/> 
        <xsl:with-param name="next" select="$next"/> 
        <xsl:with-param name="nav.context" select="$nav.context"/> 
      </xsl:call-template> 
 
      </div>

    </div>
  </div>

  <div id="footer">
    <div class="copyright">Copyright &#169; 2010-2011 Giacomo Tesio</div>
  </div>
     <xsl:call-template name="user.footer.navigation"/> 

    </body> 
  </html> 
  <xsl:value-of select="$chunk.append"/> 
</xsl:template> 

<xsl:include href="http://docbook.sourceforge.net/release/xsl/current/xhtml/chunk-code.xsl"/>

</xsl:stylesheet>
