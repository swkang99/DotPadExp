using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MainThread;
using MarkerPlugin;
using Microsoft.Office.Interop.PowerPoint;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MyUtility;
using System.IO.Ports;
using System.Threading;

namespace DotPadPlugin
{
    public class DotPadSDKPluginController
    {
        #region Member Variables
        public static int portNumber = 14;

        private static IntPtr nativeLibraryPtr;
        private static bool isInitialized = false;
        public static bool isConnected = false;
        public static bool isDisplaying = false;
        public static bool isDisplayCBRegistered = false;
        public static bool isKeyCBRegistered = false;
        //private static bool isDisplaying = true;

        public static byte[] input = new byte[] { 0xE2, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };


        public static byte[] dotpadBytes = new byte[] { 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0xF1, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8 };
        public static byte[] dotpadBytes_Static = new byte[] { 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x1F, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0xF1, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x8F, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0x88, 0xF8 };
        public static int[,] dotpadPinOverlapped = new int[60,40];
        // 콜백 함수의 시그니처를 델리게이트로 정의
        public delegate void DisplayCallbackDelegate();

        // 콜백 함수에 대한 델리게이트 핸들을 보관할 GCHandle
        private static GCHandle displayCallbackHandle;



        // 콜백 함수를 해제하는 메서드
        public static void UnregisterDisplayCallback()
        {
            // GCHandle을 해제하면서 콜백 함수 등록 해제
            if (displayCallbackHandle.IsAllocated)
            {
                displayCallbackHandle.Free();
            }
        }

        public static int padWidth = 30;
        public static int padHeight = 10;
        public static int cellWidth = 2;
        public static int cellHeight = 4;

        public static bool getIsInitialized()
        {
            return isInitialized;
        }
        public enum ERROR_CODE
        {
            DOT_ERROR_NONE = 0x00, // 성공
            HV_ERROR_UNSUPPORTED_FEATURE, // 지원하지 않는 기능

            DOT_ERROR_DOT_PAD_COULD_NOT_INIT, // DotPadSDK가 로드되지 않은 상태
            DOT_ERROR_DOT_PAD_ALREADY_INIT, // DotPadSDK가 이미 성공적으로 로드되어 있는 상태

            DOT_ERROR_TTB_DLL_LOAD_FAIL, // 점역엔진 DLL 로드 실패
            DOT_ERROR_TTB_DLL_GET_FUNC_FAIL, // 점역엔진 DLL 내 함수 포인터 획득 실패
            DOT_ERROR_TTB_DLL_COULD_NOT_LOAD, // 점역엔진 DLL 이 로드되지 않은 상태
            DOT_ERROR_TTB_COULD_NOT_SET_LANGUAGE, // 점역 엔진 언어 설정 에러

            DOT_ERROR_DISPLAY_FILE_INVALID, // 디스플레이 파일이 유효하지 아니함

            DOT_ERROR_DISPLAY_TYPE_SET, // 디스플레이 유형 설정 오류

            DOT_ERROR_HAS_NO_BRAILLE_DISPLAY, // 디바이스에 점자 출력 부분이 없는 경우

            DOT_ERROR_READ_EEPROM_PARAM, // read EEPROM parameter error
            DOT_ERROR_WRITE_EEPROM_PARAM, // write EEPROM parameter error

            DOT_ERROR_FW_BINARY_INVALID, // firmware binary is invalid

            DOT_ERROR_COM_PORT_ERROR = 0x10, // COM 포트 열기 실패
            DOT_ERROR_COM_HANDLE_INIT_ERROR, // COM 포트 핸들 초기화 실패
            DOT_ERROR_COM_PORT_ALREADY_OPENED, // COM 포트가 이미 사용중일 경우
            DOT_ERROR_COM_PORT_DISCONNECTED, // COM 포트 연결이 되어 있지 않은 상태

            DOT_ERROR_COM_WRITE_ERROR = 0x20, // COM 포트 전송 실패
            DOT_ERROR_COM_INVALID_DATA, // 유효하지 않은 데이터 값
            DOT_ERROR_COM_NOT_RESPONSE, // COM 포트로 커맨드 전송 후 응답이 없음 (3회 재시도 시 실패)
            DOT_ERROR_COM_RESPONSE_TIMEOUT, // COM 포트로 커맨드 전송 후 프로그램 오류로 인한 무한 대기를 방지하기 위한 10초 timeout 발생

            DOT_ERROR_BRAILLE_NOT_TRANSLATE = 0x40, // 점역이 안된 상태
            DOT_ERROR_KEY_OUT_OF_RANGE, // 키 범위를 벗어난 경우
            DOT_ERROR_DISPLAY_THREAD_NOT_READY, // COM 포트 오류로 display 스레드가 생성이 되지 않은 상태

            DOT_ERROR_ACCESS_INVALID_MEM = 0x80, // 유효하지 않은 메모리 접근
            DOT_ERROR_DISPLAY_IN_PROGRESS, // 점자 표시가 진행 중인 상태

            DOT_ERROR_CERTIFY_NG = 0x80, // 인증 실패
            DOT_ERROR_RESPONSE_TIMEOUT, // 응답 타임아웃

            DOT_ERROR_DISPLAY_DATA_INVALIDE_FILE, // display 데이터가 존재하지 않을 경우
            DOT_ERROR_DISPLAY_DATA_INVALIDE_LENGTH, // display 데이터 파일명의 길이가 유효하지 않을 경우
            DOT_ERROR_DISPLAY_DATA_SYNC_DATA_FAIL, // display 데이터의 SYNC 데이터 오류

            DOT_ERROR_DISPLAY_DATA_UNCHAGNED, // 현재 출력 데이터가 이전과 같은 경우
            DOT_ERROR_DISPLAY_DATA_RANGE_INVALID, // 디스플레이 데이터가 유효하지 않음

            DOT_ERROR_INVALID_DEVICE, // 유효하지 않은 장치

            DOT_ERROR_MAX
        }
        public static int getCellIndex(int x, int y)
        {
            return (y / cellHeight) * padWidth + x / cellWidth;
        }
        public static int getPinIndex(int x, int y)
        {
            return (x % cellWidth) * cellHeight + y % 4;
        }
        public static Vector2 convertDotToPinCoordinate(Vector2 pos_dot)
        {
            pos_dot = Utility.getFlooredVector(pos_dot); 
            int x = (int)pos_dot.X;
            int y = (int)pos_dot.Y;

            int ci = getCellIndex(x, y);
            int pi = getPinIndex(x, y);

            Vector2 result = new Vector2(ci, pi);
            return result;
        }
        public static void setValueAtDotCoordinate(Vector2 pos_dot, int up)
        {
            pos_dot = Utility.getFlooredVector(pos_dot);
            int x = (int)pos_dot.X;
            int y = (int)pos_dot.Y;
            int ci = getCellIndex(x, y);
            int pi = getPinIndex(x, y);

            if (((dotpadBytes_Static[ci] >> pi) & 1) == 1) return; // boundary(static) 영역은 갱신하지 않음

            bool isAlreadyUp = dotpadPinOverlapped[x, y] > 0;

            if (up == 1)
            {
                if (isAlreadyUp) dotpadPinOverlapped[x, y] = dotpadPinOverlapped[x, y] + 1; 
                else dotpadBytes[ci] |= (byte)(1 << pi);
            }
            else
            {
                if (isAlreadyUp) dotpadPinOverlapped[x, y] = dotpadPinOverlapped[x, y] - 1;
                if(dotpadPinOverlapped[x, y] == 0) dotpadBytes[ci] &= (byte)~(1 << pi);
            }
        }

        #endregion
        public static void OnApplicationQuit()
        {
            if (nativeLibraryPtr == IntPtr.Zero) return;
            Console.WriteLine("Native library is being unloaded.");
            UnregisterDisplayCallback();
            isInitialized = false;
            if (isConnected)
            {
                DotPadSDKPluginController.DOT_PAD_DEINIT_i();
                isConnected = false;
            }
            if (Native.FreeLibrary(nativeLibraryPtr))
            {
                Console.WriteLine("Native library successfully unloaded.");
            }
            else
            {
                Console.WriteLine("Native library could not be unloaded.");
            }
        }
        public static bool displayAllData()
        {
            DotPadSDKPluginController.ERROR_CODE result = DotPadSDKPluginController.DOT_PAD_DISPLAY_DATA_i(dotpadBytes, padWidth * padHeight, true);
            if (result == DotPadSDKPluginController.ERROR_CODE.DOT_ERROR_NONE)
            {
                Console.WriteLine("Data print was successful in DotPad 320 cells.");
                return true;
            }
            else
            {
                Console.WriteLine(result);
                return false;
            }
        }
        public static bool displayStaticData()
        {
            DotPadSDKPluginController.ERROR_CODE result = DotPadSDKPluginController.DOT_PAD_DISPLAY_DATA_i(dotpadBytes_Static, padWidth * padHeight, true);
            if (result == DotPadSDKPluginController.ERROR_CODE.DOT_ERROR_NONE)
            {
                Console.WriteLine("Data print was successful in DotPad 320 cells.");
                return true;
            }
            else
            {
                Console.WriteLine(result);
                return false;
            }
        }
        public static bool displayPartData(byte[] datas, int len, int startIdx)
        {
            DotPadSDKPluginController.ERROR_CODE result = DotPadSDKPluginController.DOT_PAD_DISPLAY_DATA_PART_i(datas, len, startIdx);
            if (result == DotPadSDKPluginController.ERROR_CODE.DOT_ERROR_NONE)
            {
                Console.WriteLine("Data print was successful in DotPad 320 cells.");
                return true;
            }
            else
            {
                Console.WriteLine(result);
                return false;
            }
        }
        public static bool getIsDisplaying()
        {
            
            isDisplaying = DotPadSDKPluginController.DOT_PAD_ISDISPLAYING_i();
            //if (!isDisplaying) Console.WriteLine("Display Is Updated Now!");
            
            return isDisplaying;
        }


        // Update is called once per frame
        public static void Initialization()
        {
            

            if (!isInitialized)
            {
                //if (nativeLibraryPtr == IntPtr.Zero) nativeLibraryPtr = Native.LoadLibrary("DotPadSDK-1.1.0.dll");
                if (nativeLibraryPtr == IntPtr.Zero) nativeLibraryPtr = Native.LoadLibrary("DotPadSDKDll.dll");
                if (nativeLibraryPtr == IntPtr.Zero)
                {
                    Console.WriteLine("Failed to load native library(DotPadSDKDll.dll)");
                }
                else
                {
                    Console.WriteLine("Native library(DotPadSDKDll.dll) successfully loaded.");
                    isInitialized = true;
                }
            }
            if (isInitialized && !isConnected)
            {
                // connect to dotpad
                if (!isConnected)
                {
                    ERROR_CODE result = DotPadSDKPluginController.DOT_PAD_INIT_i(portNumber);
                    if (result == ERROR_CODE.DOT_ERROR_DOT_PAD_ALREADY_INIT)
                    {
                        DotPadSDKPluginController.DOT_PAD_DEINIT_i();
                        result = DotPadSDKPluginController.DOT_PAD_INIT_i(portNumber);
                    }
                    if (result == ERROR_CODE.DOT_ERROR_NONE)
                    {
                        //result = RegisterDisplayCallback();
                        
                        if (result == ERROR_CODE.DOT_ERROR_NONE)
                        {
                            isConnected = true;
                            Console.WriteLine("Connection was successful in DotPad 320 cells to Port Number " + portNumber);
                        }
                        else
                        {
                            Console.WriteLine("Connection was failed in DotPad 320 cells to Port Number " + portNumber + "(error code=" + result + ")");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Connection was failed in DotPad 320 cells to Port Number " + portNumber + "(error code=" + result + ")");
                    }
                }
            }

            if(isInitialized && isConnected && !isDisplayCBRegistered)
            {
                ERROR_CODE result = DotPadSDKPluginController.DOT_PAD_REGISTER_DISPLAY_CALLBACK_i();
                if (result == ERROR_CODE.DOT_ERROR_NONE)
                {
                    Console.WriteLine("DOT_PAD_REGISTER_DISPLAY_CALLBACK_i is successful");
                    isDisplayCBRegistered = true;
                    isDisplaying = true;
                }
                else
                {
                    Console.WriteLine("DOT_PAD_REGISTER_DISPLAY_CALLBACK_i is failed");
                }
            }

            if(isInitialized && isConnected && isDisplayCBRegistered && !isKeyCBRegistered)
            {
                ERROR_CODE result = DotPadSDKPluginController.DOT_PAD_REGISTER_KEY_CALLBACK_i();
                if (result == ERROR_CODE.DOT_ERROR_NONE)
                {
                    Console.WriteLine("DOT_PAD_REGISTER_KEY_CALLBACK_i is successful");
                    isKeyCBRegistered = true;
                }
                else
                {
                    Console.WriteLine("DOT_PAD_REGISTER_KEY_CALLBACK_i is failed");
                }
            }

            //if(MarkerPluginController.getIsInitialized() && isInitialized && isConnected)
            //{
            //    if (Input.GetKeyDown(KeyCode.D)) // 'd' 키를 눌렀을 때
            //    {
            //        if (!isDisplaying)
            //        {
            //            //isDisplaying = true;

            //            ERROR_CODE result = DotPadSDKPluginController.DOT_PAD_DISPLAY_DATA_i(input, 10);
            //            if (result == ERROR_CODE.DOT_ERROR_NONE)
            //            {
            //                Console.WriteLine("Data print was successful in DotPad 320 cells.");
            //            }
            //            else
            //            {
            //                Console.WriteLine(result);
            //            }
            //        }
            //        else
            //        {
            //            Console.WriteLine("displaying is not completed!");
            //        }
            //    }

            //    if (Input.GetKeyDown(KeyCode.I)) // 'i' 키를 눌렀을 때
            //    {
            //        if (!isDisplaying)
            //        {
            //            //isDisplaying = true;
            //            ERROR_CODE result = DotPadSDKPluginController.DOT_PAD_RESET_DISPLAY_i();
            //            if (result == ERROR_CODE.DOT_ERROR_NONE)
            //            {
            //                Console.WriteLine("Device initialization was successful in DotPad 320 cells.");
            //            }
            //            else
            //            {
            //                Console.WriteLine(result);
            //            }
            //        }
            //        else
            //        {
            //            Console.WriteLine("displaying is not completed!");
            //        }
            //    }
            //}


        }

        #region Delegate Functions

        private delegate ERROR_CODE DOT_PAD_INIT_M(int portNumber);
        private delegate ERROR_CODE DOT_PAD_INIT_WITH_DEVICE_TYPE_M(int portNumber, int deviceType);
        private delegate ERROR_CODE DOT_PAD_DEINIT_M();
        private delegate ERROR_CODE DOT_PAD_BRAILLE_DISPLAY_M(StringBuilder input, int language);
        private delegate ERROR_CODE DOT_PAD_DISPLAY_DATA_M(byte[] data, int len, bool refresh);
        private delegate ERROR_CODE DOT_PAD_DISPLAY_DATA_PART_M(byte[] data, int len, int startIdx);
        private delegate ERROR_CODE DOT_PAD_DISPLAY_M(string filePath);
        private delegate ERROR_CODE DOT_PAD_RESET_DISPLAY_M();
        private delegate ERROR_CODE DOT_PAD_RESET_BRAILLE_DISPLAY_M();
        private delegate ERROR_CODE DOT_PAD_SEND_KEY_M(int nKeyCode);

        private delegate ERROR_CODE DOT_PAD_REGISTER_DISPLAY_CALLBACK_M();
        private delegate ERROR_CODE DOT_PAD_REGISTER_KEY_CALLBACK_M();

        private delegate bool DOT_PAD_ISDISPLAYING();
        private delegate int DOT_PAD_GET_KEY_INPUT_INDEX();
        private delegate void DOT_PAD_INIT_KEY_VARIABLES();
        #endregion


        #region Instance Functions
        public static ERROR_CODE DOT_PAD_INIT_i(int portNumber)
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_INIT_M>(nativeLibraryPtr, portNumber);
            return result;
        }
        public static ERROR_CODE DOT_PAD_INIT_WITH_DEVICE_TYPE_i(int portNumber, int deviceType)
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_INIT_WITH_DEVICE_TYPE_M>(nativeLibraryPtr, portNumber, deviceType);
            return result;
        }
        public static ERROR_CODE DOT_PAD_DEINIT_i()
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_DEINIT_M>(nativeLibraryPtr);
            return result;
        }
        public static ERROR_CODE DOT_PAD_BRAILLE_DISPLAY_i(StringBuilder input, int language)
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_BRAILLE_DISPLAY_M>(nativeLibraryPtr, input, language);
            return result;
        }
        public static ERROR_CODE DOT_PAD_DISPLAY_DATA_i(byte[] data, int len, bool refresh)
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_DISPLAY_DATA_M>(nativeLibraryPtr, data, len, refresh);
            return result;
        }
        public static ERROR_CODE DOT_PAD_DISPLAY_DATA_PART_i(byte[] data, int len, int startIdx)
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_DISPLAY_DATA_PART_M>(nativeLibraryPtr, data, len, startIdx);
            return result;
        }
        public static ERROR_CODE DOT_PAD_DISPLAY_i(string filePath)
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_DISPLAY_M>(nativeLibraryPtr, filePath);
            return result;
        }
        public static ERROR_CODE DOT_PAD_RESET_DISPLAY_i()
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_RESET_DISPLAY_M>(nativeLibraryPtr);
            return result;
        }
        public static ERROR_CODE DOT_PAD_RESET_BRAILLE_DISPLAY_i()
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_RESET_BRAILLE_DISPLAY_M>(nativeLibraryPtr);
            return result;
        }
        public static ERROR_CODE DOT_PAD_SEND_KEY_i(int nKeyCode)
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_SEND_KEY_M>(nativeLibraryPtr, nKeyCode);
            return result;
        }
        public static ERROR_CODE DOT_PAD_REGISTER_KEY_CALLBACK_i()
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_REGISTER_KEY_CALLBACK_M>(nativeLibraryPtr);
            return result;
        }
        public static ERROR_CODE DOT_PAD_REGISTER_DISPLAY_CALLBACK_i()
        {
            ERROR_CODE result = Native.Invoke<ERROR_CODE, DOT_PAD_REGISTER_DISPLAY_CALLBACK_M>(nativeLibraryPtr);
            return result;
        }



        public static bool DOT_PAD_ISDISPLAYING_i()
        {
            bool result = Native.Invoke<bool, DOT_PAD_ISDISPLAYING>(nativeLibraryPtr);
            return result;
        }
        public static int DOT_PAD_GET_KEY_INPUT_INDEX_i()
        {
            int result = Native.Invoke<int, DOT_PAD_GET_KEY_INPUT_INDEX>(nativeLibraryPtr);
            return result;
        }
        public static void DOT_PAD_INIT_KEY_VARIABLES_i()
        {
            Native.Invoke<DOT_PAD_INIT_KEY_VARIABLES>(nativeLibraryPtr);
        }
        #endregion

    }
}