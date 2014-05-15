[Code]
function IsXNAFrameworkDetected: Boolean;
var
    key: string;
    install: cardinal;
    success: boolean;
 
begin
    if IsWin64 then begin
        key := 'SOFTWARE\Wow6432Node\Microsoft\XNA\Framework\v4.0';
    end else begin
        key := 'SOFTWARE\Microsoft\XNA\Framework\v4.0';
    end;
    success := RegQueryDWordValue(HKLM, key, 'Installed', install);
    result := (success and (install = 1));
end;