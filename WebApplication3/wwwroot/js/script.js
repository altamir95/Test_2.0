var myTools = {
    returnTrueIfElemsEqual: function (one, two) {
        if (one === two) return true; else return false;
    },
    isEmpty: function (el) {
        return !$.trim(el.html())
    },
    flashingElem: function (elem) {
        $(elem).css('transition', '0.2s ').addClass('bg-danger-important');
        setTimeout(() => { $(elem).removeClass('bg-danger-important'); }, 200);
        setTimeout(() => { $(elem).removeAttr('style'); }, 400);
    },
    toSoundAudioFromFile: function (nameAudio) {
        var audio = new Audio(); // Создаём новый элемент Audio
        audio.src = "/audio/" + nameAudio + ".mp3"; // Указываем путь к звуку "клика"
        audio.play() // Автоматически запускаем
    }
};
var GamesTools = {
    activateButtonNext: function (parent) {
        $(parent + ' ' + '.next').attr('data-slide', 'next').addClass('btn-info');
    },
    createBtnsCheckPrevNextAndReturnCheck: function (appendToSelector) {
        var nextPrevPlace = $('<div class="col-12 d-flex  flex-row justify-content-between mt-5"></div>').appendTo(appendToSelector);
        var btnCheck = $('<button class="order-1 btn p-0 m-0 align-self-center rounded-circle btn-outline-none  check"></button>').appendTo(nextPrevPlace)
        var imgInBtnCheck = $('<img class="img-brightness p-0 m-0 shadow-circle-img rounded-circle" width="50" src="/img/Tick.png">').appendTo(btnCheck);
        var prevBtn = $('<button class="order-0 btn btn-info btn-sm btn-next-prev prev rounded-circle" data-slide="prev" href="#carouselExampleInterval"> ≪ </button>').appendTo(nextPrevPlace);
        var nextBtn = $('<button class="order-2 btn btn-info btn-sm next  rounded-circle btn-next-prev"  href="#carouselExampleInterval"> ≫ </button>').appendTo(nextPrevPlace);
        return btnCheck;
    }
};
//██████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
function GameMillionire(id, array) {
    var title = $('<h4 class="text-center ">Че там как там ле?</h4>').appendTo('#exercise' + id);
    var questionsPlace = $('<div class="shadow-lg"></div>').appendTo('#exercise' + id);
    var questionPlace = $('<div class="radio-style "></div>').appendTo(questionsPlace);
    $.each(array.answers, function (index, value) {
        var inputRadio = $('<input id="radio-' + index + '" type="radio" name="radio" value="' + index + '">').appendTo(questionPlace);
        var lableWithText = $('<label for="radio-' + index + '" class="m-0 border-0  p-3">' + value + '</label>').appendTo(questionPlace);
    });
    GamesTools.createBtnsCheckPrevNextAndReturnCheck('#exercise' + id).click(function () { checksWithCorrectAnswerAndIdIfItIsPossibleNext() });

    function checksWithCorrectAnswerAndIdIfItIsPossibleNext() {
        if ($('input[name="radio"]:checked').val() == array.correctAnswer) GamesTools.activateButtonNext("#exercise" + id);
        else myTools.flashingElem($("#exercise" + id + " " + ".radio-style"));
    };
}
//██████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

function GameDragAndDrop(id, array) {
    var title = $('<h3 class=" text-center">Разместите элементы правильно</h3>').appendTo('#exercise' + id);
    var columnsDrappables = $('<div style="min-height: 200px;" class=" d-flex flex-row"></div>').appendTo('#exercise' + id);
    var columnsDraggables = $('<div class=" border border-dark draggables  m-1" style="min-height:30px;"></div>').appendTo('#exercise' + id);
    var columnsBtnPrevNext = $('<div class="d-flex flex-row justify-content-between"></div>').appendTo('#exercise' + id);
    GamesTools.createBtnsCheckPrevNextAndReturnCheck('#exercise' + id).bind('click', function () { decidesPlayerLosesOrWins() });

    for (var i = 0; i < array.length; i++)$('<div class=" text-center flex-fill border border-dark bordr-dashed m-1 d-flex flex-column align-items-center  rounded-0 drappable drappable-' + i + '"></div>').appendTo(columnsDrappables);
    for (var i = 0; i < array.length; i++)$.each(array[i], function (index, value) { $('<button class="btn btn-outline-dark btn-sm rounded-0 m-1 bordr-dotted draggable">' + value + '</button>').prependTo(columnsDraggables); });

    $('.draggable').draggabilly({ containment: '#exercise' + id }).on('pointerUp', function (event, pointer) { $(".drappable").filter(function () { catchesDraggableoxInSquareDrappable(this, event.target, pointer) }); });

    function catchesDraggableoxInSquareDrappable(drappable, eventElem, pointer) {
        var drappablePos = drappable.getBoundingClientRect();
        if (pointer.pageX > drappablePos.left && pointer.pageX < drappablePos.right && pointer.pageY > drappablePos.top && pointer.pageY < drappablePos.bottom) {
            $(drappable).append(eventElem);
        } else $('.draggable').draggabilly('setPosition', 1, 1);
    };

    function decidesPlayerLosesOrWins() {
        if (myTools.isEmpty($('.draggables'))) {
            if (returnsElementsFoundOutCorrectPlace(array).length) {
                $(returnsElementsFoundOutCorrectPlace(array)).filter(function (i, elem) { myTools.flashingElem(returnsElementsFoundOutCorrectPlace(array)[i]) });
            } else {
                GamesTools.activateButtonNext("#exercise" + id);
                $('.draggable').draggabilly('disable')
            }
        } else { myTools.flashingElem('.draggables') }
    };

    function returnsElementsFoundOutCorrectPlace(array) {
        var arrayForReturn = [];
        for (var i = 0; i < $(".drappable").length; i++) {
            $(".drappable-" + i + " " + "button").filter(function (index, elem) { if ($.inArray($(this).text(), array[i]) <= -1) arrayForReturn.push($(".drappable-" + i + " " + "button").eq(index)); });
        }
        return arrayForReturn;
    };
};
//██████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

function CaruselForGames(array) {
    var carousel = $('<div id="carouselExampleInterval" class="mr-2 ml-2 carousel slide d-flex flex-column flex-grow-1 align-items-center" data-ride="carousel"></div>').appendTo('article');
    var carouselInner = $('<div class="carousel-inner  bg-white flex-grow-1" style="max-width: 500px; height: max-content;"></div>').appendTo(carousel);
    for (var i = 0; i < array.length; i++) {
        $('<div class="carousel-item   p-3 " id="exercise' + i + '"></div>').appendTo(carouselInner);
        $('.carousel-item').first().addClass('active')
    }

}
//██████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
function Dictionary(mRusWord, mArabWord) {
    this.rusWord = mRusWord;
    this.arabWord = mArabWord;
    console.log(this.rusWord);
    this.createDivWithWordsForPersonalIfTrueCourseIfFaulse = function (trueOrFalse) {
        var wordPlace = $('<div class="shadow-lg wordPlace m-1 p-2" style="width: 300px"></div>').appendTo(".wordsPlace");
        var btnPlace = $('<div class="d-flex flex-row justify-content-end" style="margin-bottom: 24px"></div>').appendTo(wordPlace);
        $('<button class="btn-volume-icon"></button>').appendTo(btnPlace).click(function () { myTools.toSoundAudioFromFile(mRusWord) });
        if (trueOrFalse) $(returnsBtnDeleteAndMakeEvent()).appendTo(btnPlace); else $(returnsBtnAddAndMakeEvent()).appendTo(btnPlace);
        $('<p class="rus">' + this.rusWord + "</p>").appendTo(wordPlace);
        $('<p class="arab" style="text-align: end">' + this.arabWord + "</p>").appendTo(wordPlace);
    }
    function returnsBtnDeleteAndMakeEvent() {
        return $('<button class="btn-delete-icon"></button>').click(function (event) { makesEventForDelete(event.target) });
    }
    function returnsBtnAddAndMakeEvent() {
        return $('<button class="btn-add-icon"></button>').click(function (event) { makesEventForAdd(event.target) });
    }
    function makesEventForDelete(elem) {
        $(elem).closest(".wordPlace").remove();
    }
    function makesEventForAdd(elem) {
        $(elem).removeClass("btn-add-icon").addClass("btn-ok-icon");
    }
}
