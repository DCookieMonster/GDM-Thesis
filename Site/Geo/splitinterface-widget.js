function SplitInterfaceWidget(init, E) {

	var selSeq = init.selSeq;
	var predSeq = init.predSeq;
	var btnNextId = init.btnNextId;
	var promptId = init.promptId;
	var progressId = init.progressId;
	var size = init.size;
	var accuType = init.accuType;
	var items = init.items;
	var categories = init.categories;
	var distractors = init.distractors;
	var ramp  = init.ramp;
	var preloadId = init.preloadId;
	var btype = init.btype;
	


	var step = 0;
	var state;
	
	var ivbonus = {}
	var ivshort = {}
	var ivuscore = {}
	var ivpctile = {}

	var startTime;
	var endTime;

	var openCat = undefined;
	var resultStream = [];
	
	var accuParts = calcAccuracy(selSeq, predSeq);

	function drawMenu() {

		var menu = $('#tb-menu');
		menu.empty();

		for(var i = 0; i < items.length; i++) {
			//conlog(predicted);
			var toolcon = $("<div class='siwidget-toolcon' >");
			toolcon.attr('id', 'header' + i)

			var catheader = $("<div class='siwidget-catheader' >");

			var catImgSrc = "icons/" + categories[i] + '/header.png'
			var catImg = $("<img>")
			catImg.attr('src', catImgSrc)
			catheader.empty().append(catImg)

			toolcon.mousedown({
				'cat' : i
			}, onHeaderClick)
			toolcon.append(catheader);
			menu.append(toolcon);

			var dropdown = $("<div class='dropdown' >");
			dropdown.attr('id', 'cat' + i)

			for(var j = 0; j < items[i].length; j++) {

				var itm = $("<img class='item'>");
				var itmId = 'itm-' + i + '-' + j;

				itm.attr('src', 'icons/' + categories[i] + '/' + items[i][j])
				itm.mousedown({
					'cat' : i,
					'itm' : j,
					'id' : itmId,
					'type' : 'reg'
				}, onItemClick)
				itm.attr('id', itmId)
				dropdown.append(itm);

			}

			menu.append(dropdown);

		}

	}

	function showCat(cat) {
		//TODO recalculate coordinates
		//conlog('show ' + cat)

		var header = $("#header" + cat);
		var dropdown = $("#cat" + cat);
		var X = header.offset().left;
		var Y = header.offset().top + header.outerHeight();

		dropdown.css('left', X);
		dropdown.css('top', Y);

		dropdown.show();
		header.css('background-color', '#aaa');

	}

	function hideCat(cat) {

		if( typeof (cat) != 'undefined') {
			$("#cat" + cat).hide();
			$("#header" + cat).css('background-color', '');
			openCat = undefined;
		}
	}

	function onHeaderClick(event) {

		if(state != 'run') {
			conlog('NOT RUNNING')
			return;
		}

		if(openCat == event.data.cat) {
			
			hideCat(openCat)
			openCat = undefined;
			return;
		}

		if( typeof (openCat) != 'undefined') {
			
			hideCat(openCat);
		}
		openCat = event.data.cat;
		showCat(openCat);
		if(openCat != selSeq[step][0]) {
			conlog("menu incorrect")
			resultStream[step].menuErrorCount += 1;
		}

		resultStream[step].startSelection = msTime();

	}

	function setPrompt(prtext) {

		var itm = $("<img>");
		itm.attr('src', "icons/" + prtext)
		$(promptId).empty().append(itm)
	}

	function clearPrompt() {
		$(promptId).empty();
	}

	function enableBtnNext() {
		$(btnNextId).removeAttr('disabled', '');
	}

	function disableBtnNext() {
		$(btnNextId).attr('disabled', 'disabled');
	}

	function enterRun(event) {
		var timems = msTime();
		state = 'run'

		//close menus if already open
		if( typeof (openCat) != 'undefined') {
			hideCat(openCat)
		}




		var cat = selSeq[step][0];
		var itm = selSeq[step][1];
		var prtext = categories[cat] + "/" + items[cat][itm];
		setPrompt(prtext);
		disableBtnNext();

		resultStream[step] = {};
		resultStream[step].startRun = timems;
		resultStream[step].menuErrorCount = 0;
		resultStream[step].itemErrorCount = 0;

		var mlog = clickString("s-c", "btn-next", "#btnnext", event, timems);
		mouselog(mlog);

	}
	
	function preloadTarget(callback){

		//preload
		var pimg = $("<img>");
		pimg.load(callback)
		var cat = selSeq[step][0]
		var itm = selSeq[step][1]
		$(preloadId).empty().append(pimg)
		pimg.attr('src', 'icons/' + categories[cat] + '/' + items[cat][itm])
				
	}

	function enterIdle() {
		state = 'idle';

		drawPred();

		preloadTarget(enableBtnNext)
		
		if(step == 0) {
			$(btnNextId).text("Start");
			$(btnNextId).addClass('btn-highlight')
		} else {
			$(btnNextId).text("Next");
			$(btnNextId).removeClass('btn-highlight')
		}

	}

	function exitIdle() {
		state = undefined;
	}

	function sortPred(pred) {
		pred = pred.dcopy();

		pred.sort(function(a, b) {
			return a[0] > b[0]
		})
	}

	function drawPred() {
		var pred = predSeq[step];
		pred = pred.dcopy();
		pred.sort()

		$("#tb-pred").empty();
		for(var i = 0; i < pred.length; i++) {
			var cat = pred[i][0];
			var itm = pred[i][1];
			var predTool = $('<div class="siwidget-toolcon">')
			var predItm = $('<div class="siw-preditem">')
			var predImg = $('<img>')
			predImg.attr('src', 'icons/' + categories[cat] + '/' + items[cat][itm])

			var itmId = 'pitm-' + cat + '-' + itm;
			predTool.attr('id', itmId)
			predTool.mousedown({
				'cat' : cat,
				'itm' : itm,
				'id' : itmId,
				'type' : 'pred'
			}, onItemClick)

			predItm.append(predImg)
			predTool.append(predItm);
			$("#tb-pred").append(predTool);
		}
	}

	function drawDistractors() {

		$("#tb-distractors-0").empty();
		for(var i = 0; i < distractors.length; i++) {

			var predTool = $('<div class="siwidget-toolcon">')
			var predItm = $('<div class="siw-preditem">')
			var predImg = $('<img>')
			predImg.attr('src', 'icons/gnome/' + distractors[i])

			predItm.append(predImg)
			predTool.append(predItm);
			$("#tb-distractors-0").append(predTool);
		}
	}

	function exitRun(event) {
		var timems = msTime();
		resultStream[step].endSelection = timems;
		resultStream[step].endRun = timems;
		//	resultStream[step].deltaSelection = resultStream[step].endSelection - resultStream[step].startSelection;
		resultStream[step].deltaSelection = resultStream[step].endRun - resultStream[step].startRun;
		resultStream[step].clickType = event.data.type;

		conlog("step: "+ step +"\tseltime: " + resultStream[step].deltaSelection)

		var mlog = clickString('s-c', 'itm-ok', "#" + event.data.id, event, timems);
		mouselog(mlog);
		step += 1;
		hideCat(openCat)
		clearPrompt();

		ivhook_shorten()

		if(step < size) {

			enterIdle();
		} else {
			finalizeBlock();
		}
	}

	function clickString(type, targname, targid, event, timems) {

		var mbn = "1";

		var targwidth = $(targid).outerWidth();
		var targheight = $(targid).outerHeight();
		var targOffsetX = ($(targid).offset()).left;
		var targOffsetY = ($(targid).offset()).top;

		var xbtnevt = event.pageX;
		var ybtnevt = event.pageY;

		var mlog = type + "," + targname + "," + mbn + "," + targwidth + "," + targheight + ",";
		mlog += targOffsetX + "," + targOffsetY + "," + xbtnevt + "," + ybtnevt + "," + timems;

		return mlog;
	}

	function onBtnNext(event) {
		exitIdle()
		//conlog("next")
		mouselog_flush();
		enterRun(event);
	}

	function onItemClick(event) {
		//conlog(event.data.cat + " " + event.data.itm)
		var cat = event.data.cat;
		var itm = event.data.itm;
 

		var targCat = selSeq[step][0];
		var targItm = selSeq[step][1];

		var selectionCorrect = (cat == targCat && itm == targItm );

		if(selectionCorrect) {
			//conlog('correct')
			exitRun(event)
		} else {
			conlog('item incorrect')
			var mlog = clickString('s-c', 'itm-err', "#" + event.data.id, event, msTime());
			resultStream[step].itemErrorCount += 1;
			mouselog(mlog);
		}
	}

	function warnIfObscured() {

		var fullyVisible = elementInViewport($("#splitinterface-widget-container")[0])

		if(!fullyVisible) {
			$(".overlay").css('width', $(document).width())
			$(".overlay").css('height', $(document).height())
			$(".overlay-error").css('padding-top', $(window).height() / 2)
			$(".overlay").show();

		} else {
			$(".overlay").hide();

		}

	}

	function calcBlockMetrics() {

		var rs = resultStream.slice(ramp)


		var sampleMean = 0;
		var sampleMax = rs[0].deltaSelection;
		var sampleMin = rs[0].deltaSelection;
		var itemErrorCount = 0;
		var menuErrorCount = 0;

		var splitClicks = 0;
		var regClicks = 0;

		for(var i = 0; i < rs.length; i++) {
			sampleMean += rs[i].deltaSelection;
			itemErrorCount += rs[i].itemErrorCount;
			menuErrorCount += rs[i].menuErrorCount;

			if(rs[i].deltaSelection > sampleMax)
				sampleMax = rs[i].deltaSelection;
			if(rs[i].deltaSelection < sampleMin)
				sampleMin = rs[i].deltaSelection;

			if(rs[i].clickType == 'pred') {
				splitClicks += 1;
			} else {
				regClicks += 1;
			}
		}
		
		var cumErrorCount = itemErrorCount + menuErrorCount

		var splitProp = splitClicks / (splitClicks + regClicks);
		sampleMean /= (rs.length);

		var devsum = 0;

		for(var i = 0; i < rs.length; i++) {
			var deviation = rs[i].deltaSelection - sampleMean;
			var sqdev = deviation * deviation;
			devsum += sqdev;
		}

		var variance = devsum / (rs.length - 1);
		var stdev = Math.sqrt(variance);

		blockTimeLen = endTime - startTime;
		
		var score = calc_score(E.scorecalc[accuType],sampleMean,cumErrorCount,stdev);
		var pctile = calc_pctile(E.scorecalc[accuType],score)
		
		conlog("score: " + score.toFixed(2) + " pctile: " + pctile)

		return {
			mean : sampleMean.toFixed(2),
			stdev : stdev.toFixed(2),
			timelen : blockTimeLen,
			min : sampleMin,
			max : sampleMax,
			ierr : itemErrorCount,
			merr : menuErrorCount,
			cerr: cumErrorCount,
			util : splitProp.toFixed(2),
			rutil : (splitProp/accuParts.avg).toFixed(2),
			rsize: rs.length,
			score: score.toFixed(2),
			pctile: pctile
		};

	}

	function finalizeBlock() {
		//$('#progress').text("Complete");
		conlog("block complete")
		$("#btnContinue").show();
		endTime = msTime();
		

		ivhook_all();
		ivhook_bonus();
		ivhook_uscore();

		var jsonlog = JSON.stringify({
			endTime : endTime
		});

		var blockResult = JSON.stringify(resultStream);
		servlog('debug', 'BlockResult', blockResult);
		servlog('debug', 'BlockEnd', jsonlog);

		var blockMetrics = calcBlockMetrics();		
		var blockSummary = JSON.stringify(blockMetrics);
		servlog('exper', 'BlockResult', blockSummary);
		servlog('exper', 'BlockEnd', jsonlog);

		var mlog = "block-end" + "," + endTime;
		mouselog(mlog);

	}

	function logBlockstart() {


		conlog(JSON.stringify(accuParts))

		var simplelog = {
			blocktype: btype,
			accutype : accuType,
			blocksize : size,
			startTime : startTime,
			accu: accuParts.avg
		}
		var debuglog = {
			blocktype : btype,
			blocksize : size,
			menuitems : items,
			selstream : selSeq,
			predstream : predSeq,
			startTime : startTime,
			accuParts: accuParts,
			accutype: accuType 
		}
		var mlog = "block-begin" + "," + size + "," + accuType + "," + startTime;

		servlog('exper', 'BlockStart', JSON.stringify(simplelog));
		servlog('debug', 'BlockStart', JSON.stringify(debuglog));
		mouselog(mlog);
	}
	
	this.getBlockMetrics = function(){
		var blockMetrics = calcBlockMetrics()
		return blockMetrics
	}
	
	this.getInit = function(){
		return init;
	}
	

	
	function ivhook_shorten(){
		
		if(btype == 'intro'){
			return
		}
		
		if(! ("ivshort" in E.cond)){
			return;
		}		

		if( step != (size - E.cond.ivshort) ) {
			return;
		}		
		
		conlog("short hook")
		
		var bm = calcBlockMetrics();
		
		ivshort.uscore = Math.round((Math.round(bm.pctile)/99.0)* E.maxuscore)
		ivshort.pctile = Math.round(bm.pctile)
		ivshort.amount = Math.round(E.cond.ivshort * ivshort.pctile  /99.0);
		
		size -= ivshort.amount;
		
		var msg_obj = {
			step : step,
			score : bm.score,
			pctile: ivshort.pctile,
			amount : ivshort.amount
		}
	
		conlog(msg_obj);
		conlog("pctile: " + ivshort.pctile  + " reduction: " + ivshort.amount);
		servlog("exper", "Shortening", JSON.stringify(msg_obj));
		
	}

	function ivhook_all(){
		
		if(btype == 'intro'){
			return
		}		
		var bm = calcBlockMetrics();
		
		ivuscore.amount = Math.round((Math.round(bm.pctile)/99.0)* E.maxuscore)
		ivpctile.amount = Math.round(bm.pctile)
		
	}
	
	function ivhook_bonus(){

		if(btype == 'intro'){
			return
		}
		
		if(! ("ivbonus" in E.cond)){
			return;
		}	
		
		var bm = calcBlockMetrics()
		ivbonus.amount = Math.round(bm.pctile * E.cond.ivbonus / 99.0)
		
		var msg_obj = {
			score : bm.score,
			pctile: bm.pctile,
			amount : ivbonus.amount,
			userid: E.userid
		}		
		
		conlog(msg_obj)
		servlog("exper", "Bonus", JSON.stringify(msg_obj));
	}

	function ivhook_uscore(){

		if(btype == 'intro'){
			return
		}
		
		if(! ("ivuscore" in E.cond)){
			return;
		}	
		
		var bm = calcBlockMetrics()

		var msg_obj = {
			score : bm.score,
			pctile: bm.pctile,
			amount: ivuscore.amount,
			userid: E.userid
		}		
		
		conlog(msg_obj)
		servlog("exper", "Uscore", JSON.stringify(msg_obj));
	}	


	
	
	this.get_size = function(){
		return size;
	}

	this.get_ivshort = function(){
		return ivshort;
	}
	
	this.get_ivbonus = function(){
		return ivbonus;
	}

	this.get_ivuscore = function(){
		return ivuscore;
	}	

	this.get_ivpctile = function(){
		return ivpctile;
	}

	drawMenu()
	drawPred();
	drawDistractors();
	$(btnNextId).unbind('click').click(onBtnNext);
	$(window).unbind('resize').resize(warnIfObscured);
	$(window).unbind('scroll').scroll(warnIfObscured);
	enterIdle();
	startTime = msTime();
	logBlockstart();

}