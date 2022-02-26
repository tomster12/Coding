# Coding

<head>
  <title>Index of /</title>
</head>

<body>
  <ul>
    {% for url in site.static_files %}
    <li><a href="{{ site.baseurl | escape }}{{ url.path | escape }}">{{ url.path | escape }}</a> </li>
    {% endfor %}
  </ul>
</body>

<h1>Index of ./</h1>
<ul></ul>

<style>body{background:#303030;margin:0;font-family:Roboto,Helvetica,Arial,sans-serif;overflow:hidden;display:flex;flex-direction:column;height:100%;width:100%}*{color:#fff;text-decoration:none}h1{background:#ff1959;font-size:1.3125rem;font-weight:500;line-height:64px;padding:0 20px;box-shadow:0 2px 4px -1px rgba(0,0,0,.2),0 4px 5px 0 rgba(0,0,0,.14),0 1px 10px 0 rgba(0,0,0,.12);margin:0}h1+ul{height:100%;padding:20px;overflow-y:auto;overflow-x:hidden}.folder>a:nth-child(1){height:15px;display:inline-block}.folder>a:nth-child(1):hover{text-decoration:underline;text-decoration-color:#ff1959}.file{position:relative;height:50px}.file a{position:absolute;display:inline-block;height:calc(100% - 20px);width:100%;font-size:1rem;font-weight:400;line-height:30px;padding:0 10px;padding-top:4px;margin:10px 0;transition:background-color .5s ease;color:#ff6892}.file:hover a{background-color:rgba(255,255,255,.1)}ul,ul.tree ul{list-style:none;margin:0;padding:0}li{display:block}ul ul{margin-left:10px}ul li{margin:0;padding:0 20px;color:#369;border-left:1px solid #ff1959}ul li:last-child{border-left:none}ul li:before{position:relative;height:28px;width:20px;color:#fff;border-bottom:1px solid #ff1959;content:"";display:inline-block;left:-20px}ul li:last-child:before{border-left:1px solid #ff1959}</style><script>var c=document.getElementsByTagName("ul")[0];function escapeHtml(e){return e.replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;").replace(/"/g,"&quot;").replace(/'/g,"&#039;")}for(var files=[{% for url in site.static_files %}'{{ url.path | replace: "'", "\'" }}',{% endfor %}],i=0;i<files.length;i++){var url=files[i].split("/").slice(1);console.group(files[i]);for(var x=0;x<url.length;x++){var file=url[x];if(file)for(var element=null,parent=null,y=0;y<x+1;y++){var el=document.querySelector("[ref="+JSON.stringify(y+"-"+url[y])+"]");if(!el){var type="folder";url[y].indexOf(".")>-1&&(type="file"),el=document.createElement("li");var a=document.createElement("a");a.innerHTML=escapeHtml(url[y]),a.href=url.slice(0,x+1).join("/"),el.appendChild(a),el.setAttribute("ref",y+"-"+url[y]),el.setAttribute("level",y),el.setAttribute("class",type),parent?parent.appendChild(el):c.appendChild(el)}parent=el}}console.groupEnd(files[i])}document.title=document.querySelector("h1").innerHTML+=" ("+files.length+" items)"</script>
