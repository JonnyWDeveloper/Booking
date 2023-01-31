// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


let createForm = document.querySelector('#createajax');
document.querySelector('#fetch').addEventListener('click', fetchCreateForm);
document.querySelector('#fetch2').addEventListener('click', fetchCreateForm2);


function removeForm() {
    // $('#createajax').html = "";
    createForm.innerHTML = "";
}

function fail(response) {
    console.log(response, 'Model error check your input');
    createForm.innerHTML = response.responseText;
}

function fixValidation() {
    const form = createForm.querySelector('form');
    $.validator.unobtrusive.parse(form);
}

function fetchCreateForm() {
    fetch('https://localhost:7108/GymClasses/FetchForm', {
        method: 'GET',
        headers: {

        }
    })
        .then(res => res.text())
        .then(data => {
            createForm.innerHTML = data;
            fixValidation();
        })
        .catch(err => console.log(err));
}

async function fetchCreateForm2() {
    const res = await fetch('https://localhost:7108/GymClasses/FetchForm', { method: 'GET' });
    //Check res! res.ok
    const data = await res.text();
    createForm.innerHTML = data;
    fixValidation();
}

$('#checkbox').click(function () {
    $('form').submit();
})