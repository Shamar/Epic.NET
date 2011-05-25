<?xml version="1.0" encoding="ASCII"?>
<!--This file was created automatically by html2xhtml-->
<!--from the HTML stylesheets.-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exsl="http://exslt.org/common" xmlns="http://www.w3.org/1999/xhtml" version="1.0" exclude-result-prefixes="exsl">

<xsl:import href="http://docbook.sourceforge.net/release/xsl/current/xhtml/docbook.xsl"/>

<xsl:import href="http://docbook.sourceforge.net/release/xsl/current/xhtml/chunk-common.xsl"/>

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
 
      <xsl:call-template name="user.footer.navigation"/> 

    </body> 
  </html> 
  <xsl:value-of select="$chunk.append"/> 
</xsl:template> 

<xsl:include href="http://docbook.sourceforge.net/release/xsl/current/xhtml/chunk-code.xsl"/>

</xsl:stylesheet>
