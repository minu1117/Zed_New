using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public static class AddressableManager
{
    // 이미지 적용
    public static async Task ApplyImage(string address, Image applyImage)
    {
        if (applyImage == null || address == null || address == string.Empty || address == "-") // 이미지가 없거나, 이미지 주소가 비었을 경우 return
            return;

        if (applyImage.sprite != null && applyImage.sprite.name == GetAddressName(address))     // 같은 이미지일 경우 return
            return;

        var loadAsync = Addressables.LoadAssetAsync<Sprite>(address);                           // 어드레서블에 저장된 이미지 로딩
        loadAsync.Completed += handle => OnImageLoaded(handle, applyImage);                     // 로딩 완료 시 이미지 적용 작업 추가

        await loadAsync.Task;
    }

    // 이미지 가져오기
    public static async Task<Sprite> GetSprite(string address)
    {
        try
        {
            var sprite = await FindAsync<Sprite>(address); // 이미지 찾기 실행
            return sprite;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception : {ex.Message}");
            return null;
        }
    }

    // 찾기
    private static async Task<T> FindAsync<T>(string address) where T : UnityEngine.Object
    {
        if (address == string.Empty)
            return null;

        try
        {
            var loadAsync = Addressables.LoadAssetAsync<T>(address);    // 어드레서블에 저장된 요소 로딩
            await loadAsync.Task;                                       // 로딩 대기

            // 로딩 완료 시 결과 return
            if (loadAsync.Status == AsyncOperationStatus.Succeeded)
            {
                return loadAsync.Result;
            }

            // 로딩 실패 시 null return
            else
            {
                Debug.LogError($"Failed to load sprite at address {address}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception : {ex.Message}");
            return null;
        }
    }

    // Dictionary에 주소와 같이 저장해 가져오기
    public static async Task<Dictionary<string, T>> LoadToDictionary<T>(List<string> addresses) where T : UnityEngine.Object
    {
        var dict = new Dictionary<string, T>();

        // 주소 List 순회
        foreach (string address in addresses)
        {
            if (dict.ContainsKey(address))                      // 같은 이름이 있을 경우 넘기기
                continue;

            T result = await FindAsync<T>(address);             // 가져오기 (대기)
            dict.Add(address, result);                          // 가져온 정보를 주소와 함께 Dictionary에 추가
        }

        return dict;    // 완료 후 return
    }

    public static async Task<Dictionary<string, T>> LoadAll<T>(List<T> loadList) where T : UnityEngine.Object
    {
        var dict = new Dictionary<string, T>();

        foreach (var element in loadList)
        {
            var key = element.name;
            if (dict.ContainsKey(key))
                continue;

            T value = await FindAsync<T>(key);
            dict.Add(key, value);
        }

        return dict;
    }

    // 로딩된 이미지 적용
    private static void OnImageLoaded(AsyncOperationHandle<Sprite> obj, Image applyImage)
    {
        // 로딩 완료 후 이미지 적용
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            applyImage.sprite = obj.Result;
        }
        else
        {
            Debug.LogError("Failed to load addressable image : private void OnImageLoaded");
        }
    }

    // 어드레서블에 저장된 이미지의 경로를 빼고, 이름만 가져오기
    private static string GetAddressName(string address)
    {
        int lastSlashIndex = address.LastIndexOf('/');                      // 마지막에 있는 '/' 문자의 인덱스 가져오기
        if (lastSlashIndex >= 0 && lastSlashIndex < address.Length - 1)     // 인덱스가 0 이상이고, 글자 총 길이보다 적을 때
        {
            return address.Substring(lastSlashIndex + 1);                   // 마지막 '/' 문자 까지 자르고, 그 다음 문자들만 return
        }

        return address; // 자르지 않아도 되는 문자이기 때문에 그대로 return
    }
}