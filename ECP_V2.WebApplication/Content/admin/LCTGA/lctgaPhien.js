$(document).ready(function () {
    let idPhienCT = 0;
    var filesArray = [];
    $('body').on('click', '.btn-close-ghiam', function (e) {
        $('#modal-GhiAmFile').modal('hide');
    });
    $('body').on('click', '.modalGhiAmFile', function (e) {
        idPhienCT = $(this).data('id');
        filesArray = [];
        $('#modal-GhiAmFile').modal('show');
    });
    async function xinQuyenGhiAm() {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            console.log("Đã cấp quyền microphone! ✅");
            stream.getTracks().forEach(track => track.stop()); // Dừng stream ngay sau khi cấp quyền
            return true;
        } catch (error) {
            console.error("Lỗi khi xin quyền ghi âm:", error);
            alert("Vui lòng cấp quyền microphone để sử dụng chức năng ghi âm!");
            return false;
        }
    }

    $("#startRecord").click(async function () {
        let check_microphone = await navigator.permissions.query({ name: "microphone" });
        if (check_microphone.state.toLowerCase() != "granted") {
            alert("Vui lòng cấp quyền microphone để sử dụng chức năng ghi âm!");
        }
        try {
            let stream = await navigator.mediaDevices.getUserMedia({ audio: true });

            console.error("Ghi âm:", stream);

            mediaRecorder = new MediaRecorder(stream);

            mediaRecorder.ondataavailable = event => {
                audioChunks.push(event.data);
            };

            mediaRecorder.onstop = () => {
                let audioBlob = new Blob(audioChunks, { type: 'audio/wav' });
                let audioUrl = URL.createObjectURL(audioBlob);
                $("#audioPlayer").attr("src", audioUrl);
                $("#uploadAudio").prop("disabled", false);
            };

            audioChunks = [];
            mediaRecorder.start();

            $("#startRecord").prop("disabled", true);
            $("#stopRecord").prop("disabled", false);
        }
        catch {
            alert("Không tìm thấy thiết bị thu âm");
        }

    });

    $("#stopRecord").click(function () {
        mediaRecorder.stop();
        $("#startRecord").prop("disabled", false);
        $("#stopRecord").prop("disabled", true);
    });


    $("#uploadAudio").click(function () {
        let audioBlob = new Blob(audioChunks, { type: 'audio/wav' });

        var $this = $(this);
        var formData = new FormData();


        formData.append('files', audioBlob, "recording.wav");



        formData.append('id', idPhienCT);

        $.ajax({
            type: 'POST',
            url: '/UploadImagePhienLV/UploadFileGhiAm',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.success) {

                    new PNotify({
                        title: 'Thông báo!',
                        text: 'Cập nhật file thành công',
                        type: 'success'
                    });
                    Paging(1, 500, $("#txtSearch").val(), $('#cmbTCPhien').val(), $('#cmbTTPhienCatDien').val(), $('#cmbTTPhienTiepDia').val(), $('#cmbTTPhienKhac').val(), $('#GioBd').val(), $('#GioKt').val(), $('#cmbDonViID').val(), $('#cmbPhongBan').val(), $('#cmbDuyetNPC').val());
                } else {
                    new PNotify({
                        title: 'Thông báo!',
                        text: 'Cập nhật file không thành công',
                        type: 'error'
                    });

                    $this.prop('disabled', false);
                }
            },
            error: function () {
                new PNotify({
                    title: 'Thông báo!',
                    text: 'Cập nhật file không thành công',
                    type: 'error'
                });

                $this.prop('disabled', false);
            }
        });

    });


    var currentSound = null;

    $("#cmbLoaiLCT").on("change", function () {
        let loai = $('#cmbLoaiLCT').val();
        if (loai == 1) {
            $(".lctga-nguoinhanlenh").removeClass("hide");
        } else {
            $(".lctga-nguoinhanlenh").addClass("hide");
        }
    });
    $(document).on("click", ".play-btn", function () {
        var songSrc = $(this).data('linkfile');
        var loai = $(this).data('loai');
        //songSrc = songSrc.replace('10.21.3.133:7077', '10.21.3.133:8998');
        var idvoice = $(this).data('id');
       
        if (currentSound) {

            currentSound.stop();
        }

        currentSound = new Howl({
            src: [songSrc],
            volume: 1.0
        });
        if (songSrc) {
            window.open(songSrc, '_blank');
        }
        else {
            alert("Kiểm tra lại đường dẫn file");
        }
        //console.log("check currentSound", currentSound)
        //if (loai == "ht") {
        //    if ($('#playvoice-ht-' + idvoice).hasClass('fa-play-circle')) {
        //        $('#playvoice-ht-' + idvoice).removeClass('fa-play-circle');
        //        $('#playvoice-ht-' + idvoice).addClass('fa-stop');
        //        if (currentSound) {

        //            currentSound.play();
        //        }
        //    } else {
        //        $('#playvoice-ht-' + idvoice).addClass('fa-play-circle');
        //        $('#playvoice-ht-' + idvoice).removeClass('fa-stop');
        //        if (currentSound) {

        //            currentSound.stop();
        //        }

        //    }
        //} else {
           
        //    if ($('#playvoice-' + idvoice).hasClass('fa-play')) {
        //        $('#playvoice-' + idvoice).removeClass('fa-play');
        //        $('#playvoice-' + idvoice).addClass('fa-stop');
        //        if (currentSound) {

        //            currentSound.play();
        //        }
        //    } else {
        //        $('#playvoice-' + idvoice).addClass('fa-play');
        //        $('#playvoice-' + idvoice).removeClass('fa-stop');
        //        if (currentSound) {

        //            currentSound.stop();
        //        }

        //    }
        //}


    });
});