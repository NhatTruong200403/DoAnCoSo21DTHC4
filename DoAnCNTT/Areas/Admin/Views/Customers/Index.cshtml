@model IList<DoAnCNTT.Models.ApplicationUser>

@{
    ViewData["Title"] = "Danh sách khách hàng";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

<h1>Danh sách khách hàng</h1>
<table class="table">
    <thead>
        <tr>
            <th>
                Khách hàng
            </th>
            <th>
                Email
            </th>
            <th>
                SĐT
            </th>
            <th>
                Báo cáo
            </th>
            <th>
                Ngày sinh
            </th>
            <th>
                Thời gian khóa
            </th>
            <th>
                Thao tác
            </th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            <tr>
                <td>
                    <a asp-area="Admin" asp-controller="Customers" asp-action="Details" asp-route-id="@item.Id"><img class="user-img-home" src="@Url.Content(item.Image)" alt="Image" />@item.Name</a>
                </td>
                <td>
                    @Html.DisplayFor(modelItem => item.Email)
                </td>
                <td>
                    @Html.DisplayFor(modelItem => item.PhoneNumber)
                </td>
                <td>
                    @Html.DisplayFor(modelItem => item.ReportPoint)
                </td>
                <td>
                    @Html.DisplayFor(modelItem => item.Birthday)
                </td>

                @{
                    if (item.LockoutEnd > DateTime.Now)
                    {
                        <td>
                            @item.LockoutEnd.Value.ToString("dd/MM/yyyy HH:mm")
                        </td>

                        <td>
                            <a asp-area="Admin" asp-controller="Customers" asp-action="UnlockAccount" asp-route-userId="@item.Id">Mở khóa</a>
                        </td>
                    }
                    else
                    {
                        <td>
                        </td>

                        <td>
                            <a asp-area="Admin" asp-controller="Customers" asp-action="LockAccount" asp-route-userId="@item.Id">Khóa tài khoản</a>
                        </td>
                    }
                }

            </tr>
        }
    </tbody>
</table>

