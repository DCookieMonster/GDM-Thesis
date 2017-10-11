//list membership
Array.prototype.has = function(v) {
	for(var i = 0; i < this.length; i++) {
		if(this[i] == v)
			return true;
	}
	return false;
}
//Knuth shuffle
function shuffle(v) {
	for(var j, x, i = v.length; i; j = parseInt(Math.random() * i), x = v[--i], v[i] = v[j], v[j] = x);
	return v;
};

//Python range
function range(k) {
	var rangearray = new Array(k);
	for(var i = 0; i < k; i++) {
		rangearray[i] = i;
	}
	return rangearray;
}

//Pylab zeros
function zeros(k) {
	var zz = new Array(k);
	for(var i = 0; i < zz.length; i++)
	zz[i] = 0;
	return zz
}

//returns an array of indices of elements with value val
Array.prototype.indicesOf = function(val) {
	var indices = [];
	for(var i = 0; i < this.length; i++) {
		if(this[i] == val)
			indices.push(i);
	}
	return indices;
}
//array deep copy
Array.prototype.dcopy = function() {
	return jQuery.extend(true, [], this);
}
//random sample
function random_k_subset(arr, k) {
	var cpy = arr.dcopy();
	var shuf = shuffle(cpy);
	var subset = shuf.slice(0, k);
	return subset;
}

function extractRandomOne(arr){
	
	var rinx = Math.floor(Math.random() * arr.length); 
	var vals = arr.splice(rinx,1);
	return vals[0];
	
}

function pickRandomOne(arr){
	var rinx = Math.floor(Math.random() * arr.length); 
	return arr[rinx]	
}

function arrays_equals_1d(a,b){
	
	if(a.length != b.length){
		return false; 
	}
	
	for (var i=0; i<a.length; i++){
		if (a[i] != b[i]){
			return false
		}
	}
	
	return true;
}

function array_1d_in_array_2d(oned, twod){
	
	if(! (twod.length >0) ){
		return false;
	}
	
	for(var i=0; i<twod.length; i++){

		if( arrays_equals_1d(twod[i],oned))
		{

			return true;
		}
	}
	
	return false;
}

//number comparator for sorting of numbers, javascript is dumb
function numerical_compare(a, b) {
	return a - b;
}

//delete a list of values from an array
Array.prototype.remove_vals = function(vals) {
	for(var i = 0; i < this.length; ) {
		var erased = false;
		for(var k = 0; k < vals.length; k++) {
			if(this[i] == vals[k]) {
				this.splice(i, 1);
				erased = true;
			}
		}
		if(!erased) {
			i++;
		}
	}
	return this;
}
//timestamp in ms
function time() {
	var t = new Date();
	return t.getTime();
}

function msTime(){
	var t = new Date();
	return t.getTime();	
}

//filter function objects from an object
//This does not make a deep copy. 
// IE 9+
function toPrintable(obj){
	
	var printable = {}
	
	var keys = Object.keys(obj);
	
	for(var i=0; i<keys.length-2; i++){
		conlog(keys[i] + " " + obj[keys[i]])
		printable[keys[i]] = obj[keys[i]];
	}
	
	return printable;
	

}

/*************** CANVAS *****************/


//draws a disc (solid color)
jQuery.fn.drawDisc = function(disc, fill_color){
	
	function _drawDisc(){
		var ctx = this.getContext('2d');
		
		ctx.fillStyle = fill_color;
		ctx.beginPath();
		ctx.arc(disc.x, disc.y, disc.radius, 0, Math.PI*2);
		ctx.closePath();		
		ctx.fill();
	
	}

	
	this.each(_drawDisc);
	
	return this; 
}

//draws multiple discs
jQuery.fn.drawDiscs = function(discs, fill_color){
	
	function _drawDiscs(){
		var ctx = this.getContext('2d');
		
		ctx.fillStyle = fill_color;
		
		for(i=0; i<discs.length; i++)
		{
			var disc = discs[i];
			ctx.beginPath();
			ctx.arc(disc.x, disc.y, disc.radius, 0, Math.PI*2);
			ctx.closePath();
			ctx.fill();	
		}
	}

	this.each(_drawDiscs);	
	return this; 
}


//draws a circle (only outline stroke)
jQuery.fn.drawCircle = function(circle, stroke_color, stroke_width){
	
	function _drawCircle(){
		var ctx = this.getContext('2d');
		
		ctx.fillStyle = stroke_color;
		ctx.lineWidth = stroke_width;
		
		ctx.beginPath();
		ctx.arc(circle.x, circle.y, circle.radius, 0, Math.PI*2);
		ctx.closePath();
		ctx.stroke();	

	}

	this.each(_drawCircle);	
	return this; 
}

//draws multiple circles
jQuery.fn.drawCircles = function(circles, stroke_color, stroke_width){
	
	function _drawCircles(){
		var ctx = this.getContext('2d');
		
		ctx.fillStyle = stroke_color;
		ctx.lineWidth = stroke_width;
		
		for(i=0; i<circles.length; i++)
		{
			var circle = circles[i];
			ctx.beginPath();
			ctx.arc(circle.x, circle.y, circle.radius, 0, Math.PI*2);
			ctx.closePath();
			ctx.stroke();	
		}

	}

	this.each(_drawCircles);	
	return this; 
}

//clears the canvas, can be optimized to save the context and remove
//any transforms before clearing
jQuery.fn.clearCanvas = function(){
	
	function _clearCanvas(){	
		var canvas = this;
		var ctx = canvas.getContext('2d');		
		ctx.clearRect(0,0,canvas.width, canvas.height);		
	}

	this.each(_clearCanvas);
	
	return this; 
}

//Splits the url variables
function getUrlVars()
{
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for(var i = 0; i < hashes.length; i++)
    {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

//transforms global coordinates to local coordinates
jQuery.fn.localCoordinates = function(event){
	
	var elem = this; 	
	var pos ={
		x: event.pageX - elem.offset().left,
		y: event.pageY - elem.offset().top
	};
	
	return pos; 
}


