
$('#summernoteNew').summernote({
    disableDragAndDrop: true,
    disableResizeImage: true,
    placeholder: 'Hello stand alone ui',
    tabsize: 2,
    height: 120,
    toolbar: [
        ['font', ['bold', 'italic', 'underline']],
        ['insert'],
    ]
});
$('#summernoteEdit').summernote({
    disableDragAndDrop: true,
    disableResizeImage: true,
    placeholder: 'Hello stand alone ui',
    tabsize: 2,
    height: 120,
    toolbar: [
        ['font', ['bold', 'italic', 'underline']],
        ['insert'],
    ]
});
$('#summernoteMove').summernote({
    disableDragAndDrop: true,
    disableResizeImage: true,
    placeholder: 'Hello stand alone ui',
    tabsize: 2,
    height: 120,
    toolbar: null
});
$('#summernoteMove').summernote('disable');