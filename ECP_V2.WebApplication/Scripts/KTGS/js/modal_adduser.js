var modal_adduser = {
    data: null,
    parrent: null,
    IDConnect: null,
    Users:null,
    init: function () {
        
    },
    show: function () {
        $('#modal_adduser').modal();
        // load nhan vien theo don vi
        service.ExcuteAjaxtGet('api/v1/DanhMuc/GetUserDV?IDConnect=' + this.IDConnect + '&madvi=' + this.data.iddonvi, null,
            (d) => {
                this.Users = d.Data;
                var data = d.Data;
                var html = '';
                for (i = 0; i < data.length; ++i) {
                    html += "<option value='" + i + "'>" + data[i].Username + "-" + data[i].TenNhanVien + "-" + data[i].ChucVu + "</option>";
                }
                $('#cbouser').html(html);                
            });        
    },
    submit() {
        var i=$('#cbouser').val();
        var url = 'api/v1/DanhMuc/AddUserDVKH?madvi=' + this.data.iddonvi +
            '&userid=' + this.Users[i].Id +
            '&id_nhom=' + this.data.ID +
            '&chucvu=' + this.Users[i].ChucVu +
            '&hoten=' + this.Users[i].TenNhanVien;
            
        service.ExcuteAjaxtGet(url, null, (d) => {
            modal_adduser.parrent.loaddata();
        }
            );    

        $('#modal_adduser').modal('hide');
    },
    cancel() {
        $('#modal_adduser').modal('hide');
    },
}
$('#cbouser').select2();
modal_adduser.init();