<?xml version="1.0" encoding="ASCII"?>
<!--This file was created automatically by html2xhtml-->
<!--from the HTML stylesheets.-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exsl="http://exslt.org/common" xmlns="http://www.w3.org/1999/xhtml" version="1.0" exclude-result-prefixes="exsl">

<xsl:import href="http://docbook.sourceforge.net/release/xsl/current/xhtml/docbook.xsl"/>

<xsl:import href="http://docbook.sourceforge.net/release/xsl/current/xhtml/chunk-common.xsl"/>

<xsl:template match="processing-instruction('asciidoc-br')">
  <br/>
</xsl:template>


<xsl:template name="user.head.content">
  <meta charset='utf-8'/>

  <link rel="icon" href="/favicon.ico" />
  <script src='../script/shCore.js' type='text/javascript'></script> 
  <script src='../script/shBrushCSharp.js' type='text/javascript'></script> 

  <link rel="alternate" type="application/atom+xml" title="Development log"  href="/atom.xml" />
  <xsl:comment><![CDATA[[if !IE 7]>
    <style type="text/css">
		#wrap {display:table;height:100%}
	</style>
  <![endif]]]></xsl:comment>
  <xsl:comment><![CDATA[[if lt IE 8]>
    <style type="text/css">
        #header {padding-top: 9px}
    </style>
  <![endif]]]></xsl:comment>
  <xsl:comment><![CDATA[[if IE 8]>
    <style type="text/css">
		#wrap {width:100%}
	</style>
  <![endif]]]></xsl:comment>
  <script type="text/javascript">

  var _gaq = _gaq || [];
  _gaq.push(['_setAccount', 'UA-23846269-1']);
  _gaq.push(['_trackPageview']);

  (function() {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
  })();

  </script>
</xsl:template>

<xsl:template match="programlisting|screen|synopsis">
  <xsl:param name="suppress-numbers" select="'0'"/>
  <xsl:variable name="id">
    <xsl:call-template name="object.id"/>
  </xsl:variable>

  <xsl:call-template name="anchor"/>

  <xsl:if test="$shade.verbatim != 0">
    <xsl:message>
      <xsl:text>The shade.verbatim parameter is deprecated. </xsl:text>
      <xsl:text>Use CSS instead,</xsl:text>
    </xsl:message>
    <xsl:message>
      <xsl:text>for example: pre.</xsl:text>
      <xsl:value-of select="local-name(.)"/>
      <xsl:text> { background-color: #E0E0E0; }</xsl:text>
    </xsl:message>
  </xsl:if>

  <xsl:choose>
    <xsl:when test="$suppress-numbers = '0'
                    and @linenumbering = 'numbered'
                    and $use.extensions != '0'
                    and $linenumbering.extension != '0'">
      <xsl:variable name="rtf">
        <xsl:choose>
          <xsl:when test="$highlight.source != 0">
            <xsl:call-template name="apply-highlighting"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:apply-templates/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>
      <script type="syntaxhighlighter" class="brush: csharp">
        <xsl:if test="@width != ''">
          <xsl:attribute name="width">
            <xsl:value-of select="@width"/>
          </xsl:attribute>
        </xsl:if>
        <xsl:call-template name="number.rtf.lines">
          <xsl:with-param name="rtf" select="$rtf"/>
        </xsl:call-template>
      </script>
    </xsl:when>
    <xsl:otherwise>
      <script type="syntaxhighlighter" class="brush: csharp">
        <xsl:if test="@width != ''">
          <xsl:attribute name="width">
            <xsl:value-of select="@width"/>
          </xsl:attribute>
        </xsl:if>
        <xsl:choose>
          <xsl:when test="$highlight.source != 0">
            <xsl:call-template name="apply-highlighting"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:apply-templates/>
          </xsl:otherwise>
        </xsl:choose>
      </script>
    </xsl:otherwise>
  </xsl:choose>
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
                  <a href="/doc/manual.html" class="toc-title">
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
  <xsl:param name="filename"/> 
  <xsl:param name="nav.context"/> 
  <xsl:param name="content"> 
    <xsl:apply-imports/> 
  </xsl:param> 
 
  <xsl:call-template name="user.preroot"/> 
 
  <html> 
    <meta http-equiv="cache-control" content="no-cache" />
    <xsl:call-template name="html.head"> 
      <xsl:with-param name="prev" select="$prev"/> 
      <xsl:with-param name="next" select="$next"/> 
    </xsl:call-template> 
 
    <body> 
      <xsl:call-template name="body.attributes"/> 

  <div id="wrap">
    <div id="header">
      <a href="https://github.com/bards/Epic.NET"><img style="position: absolute; top: 0; left: 0; border: 0;" src="http://s3.amazonaws.com/github/ribbons/forkme_left_orange_ff7600.png" alt="Fork me on GitHub" /></a>
      <a class="title" href="/">Epic</a>
      <span>dominant domains</span>
    </div>
    <div id="main">
      <div id="content">
      <xsl:if test="$filename = 'manual.html'">
        <ul class="menu">
          <li><a href="/index.html">Overview</a></li>
          <li><a href="/roadmap.html">Roadmap</a></li>
          <li><a href="/blog.html" title="Development blog">Blog</a></li>
          <li><a href="/doc/index.html">Documentation</a></li>
          <li><a href="https://github.com/bards/Epic.NET/archives/master">Download</a></li>
          <li><a href="/license.html">License</a></li>
          <li><a href="http://epic.tesio.it/credits.html">Bards</a></li>
        </ul>
      </xsl:if>
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
    <div class="copyright">Copyright &#169; 2010-2012 Giacomo Tesio</div>
  </div>
     <xsl:call-template name="user.footer.navigation"/> 
  <script type='text/javascript'>SyntaxHighlighter.all()</script> 
    </body> 
  </html> 
  <xsl:value-of select="$chunk.append"/> 
</xsl:template> 

<xsl:template name="gentext.nav.home">
  <xsl:call-template name="gentext">
    <xsl:with-param name="key">TableofContents</xsl:with-param>
  </xsl:call-template>
</xsl:template>

<xsl:template name="process-chunk">
  <xsl:param name="prev" select="."/>
  <xsl:param name="next" select="."/>
  <xsl:param name="content">
    <xsl:apply-imports/>
  </xsl:param>

  <xsl:variable name="ischunk">
    <xsl:call-template name="chunk"/>
  </xsl:variable>

  <xsl:variable name="chunkfn">
    <xsl:if test="$ischunk='1'">
      <xsl:apply-templates mode="chunk-filename" select="."/>
    </xsl:if>
  </xsl:variable>

  <xsl:if test="$ischunk='0'">
    <xsl:message>
      <xsl:text>Error </xsl:text>
      <xsl:value-of select="name(.)"/>
      <xsl:text> is not a chunk!</xsl:text>
    </xsl:message>
  </xsl:if>

  <xsl:variable name="filename">
    <xsl:call-template name="make-relative-filename">
      <xsl:with-param name="base.dir" select="$base.dir"/>
      <xsl:with-param name="base.name" select="$chunkfn"/>
    </xsl:call-template>
  </xsl:variable>

  <xsl:call-template name="write.chunk">
    <xsl:with-param name="filename" select="$filename"/>
    <xsl:with-param name="content">
      <xsl:call-template name="chunk-element-content">
        <xsl:with-param name="prev" select="$prev"/>
        <xsl:with-param name="next" select="$next"/>
        <xsl:with-param name="filename" select="$chunkfn"/>
        <xsl:with-param name="content" select="$content"/>
      </xsl:call-template>
    </xsl:with-param>
    <xsl:with-param name="quiet" select="$chunk.quietly"/>
  </xsl:call-template>
</xsl:template>

<xsl:template match="footnote">
 <xsl:variable name="name">
   <xsl:call-template name="object.id"/>
 </xsl:variable>
 <xsl:variable name="href">
   <xsl:text>#ftn.</xsl:text>
   <xsl:call-template name="object.id"/>
 </xsl:variable>
 
 <xsl:choose>
   <xsl:when test="ancestor::tgroup">
     <sup class="footnoteref">
       <xsl:text>[</xsl:text>
       <a name="{$name}" href="{$href}">
         <xsl:apply-templates select="." mode="class.attribute"/>
         <xsl:apply-templates select="." mode="footnote.number"/>
       </a>
       <xsl:text>]</xsl:text>
     </sup>
   </xsl:when>
   <xsl:otherwise>
     <sup class="footnoteref">
       <xsl:text>[</xsl:text>
       <a name="{$name}" href="{$href}">
         <xsl:apply-templates select="." mode="class.attribute"/>
         <xsl:apply-templates select="." mode="footnote.number"/>
       </a>
       <xsl:text>]</xsl:text>
     </sup>
   </xsl:otherwise>
 </xsl:choose>
</xsl:template>

<xsl:template match="footnote/para[1]|footnote/simpara[1]" priority="2">
 <!-- this only works if the first thing in a footnote is a para, -->
 <!-- which is ok, because it usually is. -->
 <xsl:variable name="name">
   <xsl:text>ftn.</xsl:text>
   <xsl:call-template name="object.id">
     <xsl:with-param name="object" select="ancestor::footnote"/>
   </xsl:call-template>
 </xsl:variable>
 <xsl:variable name="href">
   <xsl:text>#</xsl:text>
   <xsl:call-template name="object.id">
     <xsl:with-param name="object" select="ancestor::footnote"/>
   </xsl:call-template>
 </xsl:variable>
 <p>
   <xsl:if test="@role and $para.propagates.style != 0">
     <xsl:apply-templates select="." mode="class.attribute">
       <xsl:with-param name="class" select="@role"/>
     </xsl:apply-templates>
   </xsl:if>
   <sup class="footnote">
     <xsl:text>[</xsl:text>
     <a name="{$name}" href="{$href}">
       <xsl:apply-templates select="." mode="class.attribute"/>
       <xsl:apply-templates select="ancestor::footnote"
                            mode="footnote.number"/>
     </a>
     <xsl:text>] </xsl:text>
   </sup>
   <xsl:apply-templates/>
 </p>
</xsl:template>

<xsl:include href="http://docbook.sourceforge.net/release/xsl/current/xhtml/chunk-code.xsl"/>

</xsl:stylesheet>
