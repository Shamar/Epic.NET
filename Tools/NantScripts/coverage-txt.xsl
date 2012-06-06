<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
    <xsl:output
        method="text"
        indent="no"/>
    <xsl:strip-space elements="*"/>
	<xsl:template match="//method[seqpnt[@visitcount='0']]">
	    <xsl:value-of select="@class" />,<xsl:value-of select="@name" />
	    <xsl:text>&#10;</xsl:text>
	</xsl:template>
</xsl:stylesheet>
